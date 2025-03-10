﻿using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_Vue.WebAPI.Repositories.Classes
{
    public class FavoritePlayerAdvertisementRepository : IFavoritePlayerAdvertisementRepository
    {
        private readonly AppDbContext _dbContext;

        public FavoritePlayerAdvertisementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddToFavorites(FavoritePlayerAdvertisement favoritePlayerAdvertisement)
        {
            await _dbContext.FavoritePlayerAdvertisements.AddAsync(favoritePlayerAdvertisement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteFromFavorites(int favoritePlayerAdvertisementId)
        {
            var favoritePlayerAdvertisement = await _dbContext.FavoritePlayerAdvertisements.FindAsync(favoritePlayerAdvertisementId);
            if (favoritePlayerAdvertisement == null)
                throw new ArgumentException($"No Favorite Player Advertisement found with ID {favoritePlayerAdvertisementId}");

            _dbContext.FavoritePlayerAdvertisements.Remove(favoritePlayerAdvertisement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CheckPlayerAdvertisementIsFavorite(int playerAdvertisementId, string userId)
        {
            var isFavorite = await _dbContext.FavoritePlayerAdvertisements
                .FirstOrDefaultAsync(pa => pa.PlayerAdvertisementId == playerAdvertisementId && pa.UserId == userId);

            return isFavorite?.Id ?? 0;
        }
    }
}