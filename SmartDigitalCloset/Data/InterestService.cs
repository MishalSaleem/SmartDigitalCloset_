using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using System.Linq;

namespace SmartDigitalCloset.Data
{
    public class InterestService
    {
        private readonly ILocalStorageService _localStorage;
        
        public InterestService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
          public async Task SaveInterestAsync(string userEmail, string interestName)
        {
            try
            {
                string emailKey = userEmail.Replace("@", "_").Replace(".", "_");
                
                // Get current list of interests
                var interests = await _localStorage.GetItemAsync<List<string>>($"userInterests_{emailKey}") ?? new List<string>();
                
                // Add the interest if not already in the list
                if (!interests.Contains(interestName, StringComparer.OrdinalIgnoreCase))
                {
                    interests.Add(interestName);
                    await _localStorage.SetItemAsync($"userInterests_{emailKey}", interests);
                    
                    // Update the count in localStorage
                    await _localStorage.SetItemAsync($"interestsCount_{emailKey}", interests.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving interest: {ex.Message}");
            }
        }
          public async Task RemoveInterestAsync(string userEmail, string interestName)
        {
            try
            {
                string emailKey = userEmail.Replace("@", "_").Replace(".", "_");
                
                // Get current list of interests
                var interests = await _localStorage.GetItemAsync<List<string>>($"userInterests_{emailKey}") ?? new List<string>();
                
                // Remove the interest if it exists
                interests.RemoveAll(i => i.Equals(interestName, StringComparison.OrdinalIgnoreCase));
                
                // Update the list and count
                await _localStorage.SetItemAsync($"userInterests_{emailKey}", interests);
                await _localStorage.SetItemAsync($"interestsCount_{emailKey}", interests.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing interest: {ex.Message}");
            }
        }
          public async Task<List<string>> GetInterestsAsync(string userEmail)
        {
            try
            {
                string emailKey = userEmail.Replace("@", "_").Replace(".", "_");
                return await _localStorage.GetItemAsync<List<string>>($"userInterests_{emailKey}") ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting interests: {ex.Message}");
                return new List<string>();
            }
        }
        
        public async Task<bool> HasInterestAsync(string userEmail, string interestName)
        {
            try
            {
                var interests = await GetInterestsAsync(userEmail);
                return interests.Contains(interestName, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
