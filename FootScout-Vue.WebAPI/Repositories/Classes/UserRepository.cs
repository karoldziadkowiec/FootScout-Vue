using AutoMapper;
using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FootScout_Vue.WebAPI.Repositories.Classes
{
    // Repozytorium z zaimplementowanymi metodami związanymi z użytkownikiem
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserRepository(AppDbContext dbContext, IMapper mapper, UserManager<User> userManager, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        // Zwróć konkretnego użytkownika
        public async Task<UserDTO> GetUser(string userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            var userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;
        }

        // Zwróć wszystkich użytkowników
        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            var users = await _dbContext.Users.OrderByDescending(u => u.CreationDate).ToListAsync();
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
            return userDTOs;
        }

        // Zwróć użytkowników dla roli User
        public async Task<IEnumerable<UserDTO>> GetOnlyUsers()
        {
            var onlyUsers = await _userManager.GetUsersInRoleAsync("User");
            var sortedUsers = onlyUsers.OrderByDescending(u => u.CreationDate);
            var onlyUserDTOs = _mapper.Map<IEnumerable<UserDTO>>(sortedUsers);
            return onlyUserDTOs;
        }

        // Zwróć użytkowników dla roli Admin
        public async Task<IEnumerable<UserDTO>> GetOnlyAdmins()
        {
            var onlyAdmins = await _userManager.GetUsersInRoleAsync("Admin");
            var sortedUsers = onlyAdmins.OrderByDescending(u => u.CreationDate);
            var onlyAdminDTOs = _mapper.Map<IEnumerable<UserDTO>>(sortedUsers);
            return onlyAdminDTOs;
        }

        // Zwróć rolę dla konkretnego użytkownika
        public async Task<string> GetUserRole(string userId)
        {
            return await (from ur in _dbContext.UserRoles
                                  join r in _dbContext.Roles on ur.RoleId equals r.Id
                                  where ur.UserId == userId
                                  select r.Name).FirstOrDefaultAsync();
        }

        // Zwróć liczbę wszystkich użytkowników
        public async Task<int> GetUserCount()
        {
            return await _dbContext.Users.CountAsync();
        }

        // Zaktualizuj konkretnego użytkownika 
        public async Task UpdateUser(string userId, UserUpdateDTO dto)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                _mapper.Map(dto, user);
                _dbContext.Entry(user).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
        }

        // Zresetuj hasło dla konkretnego użytkownika 
        public async Task ResetUserPassword(string userId, UserResetPasswordDTO dto)
        {
            if (!dto.PasswordHash.Equals(dto.ConfirmPasswordHash))
                throw new ArgumentException($"Confirmed password is different.");

            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                _mapper.Map(dto, user);

                if (!string.IsNullOrEmpty(dto.PasswordHash))
                    user.PasswordHash = _passwordHasher.HashPassword(user, dto.PasswordHash);

                _dbContext.Entry(user).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
        }

        // Usuń konkretnego użytkownika
        public async Task DeleteUser(string userId)
        {
            // znajdź użytkownika
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            // znajdź i usuń historie klubowe użytkownika
            var clubHistories = await _dbContext.ClubHistories
                .Where(ch => ch.PlayerId == userId)
                .ToListAsync();

            foreach (var clubHistory in clubHistories)
            {
                if (clubHistory.AchievementsId != null)
                {
                    var achievements = await _dbContext.Achievements.FindAsync(clubHistory.AchievementsId);
                    if (achievements != null)
                        _dbContext.Achievements.Remove(achievements);
                }
            }
            _dbContext.ClubHistories.RemoveRange(clubHistories);

            // znajdź i usuń czaty i wiadomości użytkownika
            var chats = await _dbContext.Chats
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .ToListAsync();

            foreach (var chat in chats)
            {
                if (chat.User1Id != null && chat.User2Id != null)
                {
                    var messages = await _dbContext.Messages
                        .Where(m => m.ChatId == chat.Id)
                        .ToListAsync();

                    if (messages.Any())
                        _dbContext.Messages.RemoveRange(messages);
                }
            }
            _dbContext.Chats.RemoveRange(chats);

            // znajdź i usuń ulubione ogłoszenia użytkownika
            var playerFavorites = await _dbContext.FavoritePlayerAdvertisements
                    .Where(fpa => fpa.UserId == userId)
                    .ToListAsync();
            _dbContext.FavoritePlayerAdvertisements.RemoveRange(playerFavorites);

            // znajdź użytkownika unkown (do przypisania niektórych zasobów)
            var unknownUser = await _dbContext.Users
               .Where(u => u.Email == "unknown@unknown.com")
               .SingleOrDefaultAsync();

            if (unknownUser == null)
                throw new InvalidOperationException("Unknown user not found");

            var unknownUserId = unknownUser.Id;

            var offeredStatus = await _dbContext.OfferStatuses
                .FirstOrDefaultAsync(a => a.StatusName == "Offered");
            var rejectedStatus = await _dbContext.OfferStatuses
                .FirstOrDefaultAsync(a => a.StatusName == "Rejected");

            var playerAdvertisements = await _dbContext.PlayerAdvertisements
               .Where(pa => pa.PlayerId == userId)
               .ToListAsync();

            foreach (var advertisement in playerAdvertisements)
            {
                advertisement.EndDate = DateTime.Now;
                advertisement.PlayerId = unknownUserId;
            }

            var clubOffers = await _dbContext.ClubOffers
               .Where(co => co.ClubMemberId == userId)
               .ToListAsync();

            foreach (var offer in clubOffers)
            {
                if(offer.OfferStatusId == offeredStatus.Id)
                {
                    offer.OfferStatusId = rejectedStatus.Id;
                }
                offer.ClubMemberId = unknownUserId;
            }

            // zwróć i usuń wszystkie problemy
            var problems = await _dbContext.Problems
                .Where(p => p.RequesterId == userId)
                .ToListAsync();
            _dbContext.Problems.RemoveRange(problems);

            // finalnie usuń użytkownika
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        // Zwróć historię klubową dla konkretnego użytkownika
        public async Task<IEnumerable<ClubHistory>> GetUserClubHistory(string userId)
        {
            return await _dbContext.ClubHistories
                .Include(ch => ch.Achievements)
                .Include(ch => ch.PlayerPosition)
                .Include(ch => ch.Player)
                .Where(ch => ch.PlayerId == userId)
                .OrderByDescending(ch => ch.StartDate)
                .ThenByDescending(ch => ch.EndDate)
                .ToListAsync();
        }

        // Zwróć ogłoszenia dla konkretnego użytkownika
        public async Task<IEnumerable<PlayerAdvertisement>> GetUserPlayerAdvertisements(string userId)
        {
            return await _dbContext.PlayerAdvertisements
                .Include(pa => pa.PlayerPosition) // podepnij pozycję dla tego ogłoszenia
                .Include(pa => pa.PlayerFoot) // podepnij nogę dla tego ogłoszenia
                .Include(pa => pa.SalaryRange) // podepnij widełki płacowe dla tego ogłoszenia
                .Include(pa => pa.Player) // podepnij użytkownika dla tego ogłoszenia
                .Where(pa => pa.PlayerId == userId) // znajdź użytkownika dla tego id
                .OrderByDescending(pa => pa.EndDate) // posortu malejąco bazując na dacie
                .ToListAsync(); // zapisz to listy
        }

        // Zwróć aktywne ogłoszenia dla konkretnego użytkownika
        public async Task<IEnumerable<PlayerAdvertisement>> GetUserActivePlayerAdvertisements(string userId)
        {
            return await _dbContext.PlayerAdvertisements
                .Include(pa => pa.PlayerPosition)
                .Include(pa => pa.PlayerFoot)
                .Include(pa => pa.SalaryRange)
                .Include(pa => pa.Player)
                .Where(pa => pa.PlayerId == userId && pa.EndDate >= DateTime.Now)
                .OrderBy(pa => pa.EndDate)
                .ToListAsync();
        }

        // Zwróć nieaktywne ogłoszenia dla konkretnego użytkownika
        public async Task<IEnumerable<PlayerAdvertisement>> GetUserInactivePlayerAdvertisements(string userId)
        {
            return await _dbContext.PlayerAdvertisements
                .Include(pa => pa.PlayerPosition)
                .Include(pa => pa.PlayerFoot)
                .Include(pa => pa.SalaryRange)
                .Include(pa => pa.Player)
                .Where(pa => pa.PlayerId == userId && pa.EndDate < DateTime.Now)
                .OrderByDescending(pa => pa.EndDate)
                .ToListAsync();
        }

        // Zwróć ulubione ogłoszenia dla konkretnego użytkownika
        public async Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserFavoritePlayerAdvertisements(string userId)
        {
            return await _dbContext.FavoritePlayerAdvertisements
                .Include(pa => pa.PlayerAdvertisement)
                .Include(pa => pa.PlayerAdvertisement.PlayerPosition)
                .Include(pa => pa.PlayerAdvertisement.PlayerFoot)
                .Include(pa => pa.PlayerAdvertisement.SalaryRange)
                .Include(pa => pa.PlayerAdvertisement.Player)
                .Include(pa => pa.User)
                .Where(pa => pa.UserId == userId)
                .OrderByDescending(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();
        }

        // Zwróć aktywne ulubione ogłoszenia dla konkretnego użytkownika
        public async Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserActiveFavoritePlayerAdvertisements(string userId)
        {
            return await _dbContext.FavoritePlayerAdvertisements
                .Include(pa => pa.PlayerAdvertisement)
                .Include(pa => pa.PlayerAdvertisement.PlayerPosition)
                .Include(pa => pa.PlayerAdvertisement.PlayerFoot)
                .Include(pa => pa.PlayerAdvertisement.SalaryRange)
                .Include(pa => pa.PlayerAdvertisement.Player)
                .Include(pa => pa.User)
                .Where(pa => pa.UserId == userId && pa.PlayerAdvertisement.EndDate >= DateTime.Now)
                .OrderBy(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();
        }

        // Zwróć nieaktywne ulubione dla konkretnego użytkownika
        public async Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserInactiveFavoritePlayerAdvertisements(string userId)
        {
            return await _dbContext.FavoritePlayerAdvertisements
                .Include(pa => pa.PlayerAdvertisement)
                .Include(pa => pa.PlayerAdvertisement.PlayerPosition)
                .Include(pa => pa.PlayerAdvertisement.PlayerFoot)
                .Include(pa => pa.PlayerAdvertisement.SalaryRange)
                .Include(pa => pa.PlayerAdvertisement.Player)
                .Include(pa => pa.User)
                .Where(pa => pa.UserId == userId && pa.PlayerAdvertisement.EndDate < DateTime.Now)
                .OrderByDescending(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();
        }

        // Zwróć otrzymane oferty dla konkretnego użytkownika
        public async Task<IEnumerable<ClubOffer>> GetReceivedClubOffers(string userId)
        {
            return await _dbContext.ClubOffers
                .Include(co => co.PlayerAdvertisement)
                .Include(pa => pa.PlayerAdvertisement.PlayerPosition)
                .Include(pa => pa.PlayerAdvertisement.PlayerFoot)
                .Include(pa => pa.PlayerAdvertisement.SalaryRange)
                .Include(pa => pa.PlayerAdvertisement.Player)
                .Include(co => co.OfferStatus)
                .Include(co => co.PlayerPosition)
                .Include(co => co.ClubMember)
                .Where(pa => pa.PlayerAdvertisement.PlayerId == userId)
                .OrderByDescending(pa => pa.PlayerAdvertisement.EndDate)
                .ToListAsync();
        }

        // Zwróć wysłane oferty dla konkretnego użytkownika
        public async Task<IEnumerable<ClubOffer>> GetSentClubOffers(string userId)
        {
            return await _dbContext.ClubOffers
                .Include(co => co.PlayerAdvertisement)
                .Include(pa => pa.PlayerAdvertisement.PlayerPosition)
                .Include(pa => pa.PlayerAdvertisement.PlayerFoot)
                .Include(pa => pa.PlayerAdvertisement.SalaryRange)
                .Include(pa => pa.PlayerAdvertisement.Player)
                .Include(co => co.OfferStatus)
                .Include(co => co.PlayerPosition)
                .Include(co => co.ClubMember)
                .Where(pa => pa.ClubMemberId == userId)
                .OrderByDescending(pa => pa.PlayerAdvertisement.CreationDate)
                .ToListAsync();
        }

        // Zwróć wszystkie czaty dla konkretnego użytkownika
        public async Task<IEnumerable<Chat>> GetUserChats(string userId)
        {
            var chats = await _dbContext.Chats
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .ToListAsync();

            var chatWithLastMessageTimestamps = new List<(Chat Chat, DateTime? LastMessageTimestamp)>();
            foreach (var chat in chats)
            {
                var lastMessageTimestamp = await _dbContext.Messages
                    .Where(m => m.ChatId == chat.Id)
                    .OrderByDescending(m => m.Timestamp)
                    .Select(m => (DateTime?)m.Timestamp)
                    .FirstOrDefaultAsync();
                chatWithLastMessageTimestamps.Add((chat, lastMessageTimestamp));
            }

            return chatWithLastMessageTimestamps
                .OrderByDescending(c => c.LastMessageTimestamp)
                .Select(c => c.Chat)
                .ToList();
        }

        // Eksportuj użytkowników do pliku .csv
        public async Task<MemoryStream> ExportUsersToCsv()
        {
            var users = await GetUsers();
            var csv = new StringBuilder();
            csv.AppendLine("E-mail,First Name,Last Name,Phone Number,Location,Creation Date");

            foreach (var user in users)
            {
                csv.AppendLine($"{user.Email},{user.FirstName},{user.LastName},{user.PhoneNumber},{user.Location},{user.CreationDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}