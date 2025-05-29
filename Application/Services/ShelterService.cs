using SCED.API.Application.Interfaces;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Interfaces;

namespace SCED.API.Application.Services
{
    public class ShelterService : IShelterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShelterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<Shelter>> GetAllSheltersAsync()
        {
            try
            {
                return await _unitOfWork.Shelters.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar todos os abrigos.", ex);
            }
        }

        public async Task<Shelter?> GetShelterByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do abrigo deve ser maior que zero.", nameof(id));

            try
            {
                return await _unitOfWork.Shelters.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar abrigo com ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Shelter>> GetAvailableSheltersAsync()
        {
            try
            {
                return await _unitOfWork.Shelters.GetAvailableSheltersAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar abrigos disponíveis.", ex);
            }
        }

        public async Task<IEnumerable<Shelter>> GetNearbySheltersAsync(double latitude, double longitude, double radiusKm = 10.0)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("A latitude deve estar entre -90 e 90 graus.", nameof(latitude));
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("A longitude deve estar entre -180 e 180 graus.", nameof(longitude));
            if (radiusKm <= 0 || radiusKm > 1000)
                throw new ArgumentException("O raio deve estar entre 0 e 1000 km.", nameof(radiusKm));

            try
            {
                return await _unitOfWork.Shelters.GetSheltersInRadiusAsync(latitude, longitude, radiusKm);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar abrigos na área especificada.", ex);
            }
        }

        public async Task<IEnumerable<Shelter>> GetSheltersByCapacityRangeAsync(int minCapacity, int maxCapacity)
        {
            if (minCapacity < 0)
                throw new ArgumentException("A capacidade mínima não pode ser negativa.", nameof(minCapacity));
            if (maxCapacity <= minCapacity)
                throw new ArgumentException("A capacidade máxima deve ser maior que a mínima.", nameof(maxCapacity));

            try
            {
                return await _unitOfWork.Shelters.GetSheltersByCapacityRangeAsync(minCapacity, maxCapacity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar abrigos por faixa de capacidade.", ex);
            }
        }

        public async Task<Shelter> CreateShelterAsync(Shelter shelter)
        {
            if (shelter == null)
                throw new ArgumentNullException(nameof(shelter), "O abrigo não pode ser nulo.");

            ValidateShelter(shelter);

            if (shelter.Capacity <= 0)
                throw new ArgumentException("A capacidade deve ser maior que zero.", nameof(shelter.Capacity));
            
            if (shelter.CurrentOccupancy < 0)
                shelter.CurrentOccupancy = 0;
            
            if (shelter.CurrentOccupancy > shelter.Capacity)
                throw new ArgumentException("A ocupação atual não pode ser maior que a capacidade.", nameof(shelter.CurrentOccupancy));

            try
            {
                await _unitOfWork.Shelters.AddAsync(shelter);
                await _unitOfWork.SaveChangesAsync();
                return shelter;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao salvar o abrigo no banco de dados.", ex);
            }
        }

        public async Task<Shelter?> UpdateShelterAsync(long id, Shelter updatedShelter)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do abrigo deve ser maior que zero.", nameof(id));
            if (updatedShelter == null)
                throw new ArgumentNullException(nameof(updatedShelter), "O abrigo atualizado não pode ser nulo.");
            if (id != updatedShelter.Id)
                throw new ArgumentException("O ID fornecido não corresponde ao ID do abrigo.", nameof(id));

            ValidateShelter(updatedShelter);

            try
            {
                var existingShelter = await _unitOfWork.Shelters.GetByIdAsync(id);
                if (existingShelter == null)
                    return null;

                existingShelter.Name = updatedShelter.Name;
                existingShelter.Address = updatedShelter.Address;
                existingShelter.Capacity = updatedShelter.Capacity;
                existingShelter.CurrentOccupancy = updatedShelter.CurrentOccupancy;
                existingShelter.Latitude = updatedShelter.Latitude;
                existingShelter.Longitude = updatedShelter.Longitude;

                await _unitOfWork.Shelters.UpdateAsync(existingShelter);
                await _unitOfWork.SaveChangesAsync();
                return existingShelter;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar o abrigo com ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteShelterAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do abrigo deve ser maior que zero.", nameof(id));

            try
            {
                var shelter = await _unitOfWork.Shelters.GetByIdAsync(id);
                if (shelter == null)
                    return false;

                await _unitOfWork.Shelters.DeleteAsync(shelter);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao deletar o abrigo com ID {id}.", ex);
            }
        }

        public async Task<bool> UpdateCapacityAsync(long id, int newCurrentOccupancy)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do abrigo deve ser maior que zero.", nameof(id));
            if (newCurrentOccupancy < 0)
                throw new ArgumentException("A ocupação atual não pode ser negativa.", nameof(newCurrentOccupancy));

            try
            {
                bool result = await _unitOfWork.Shelters.UpdateOccupancyAsync(id, newCurrentOccupancy);
                if (result)
                    await _unitOfWork.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar ocupação do abrigo com ID {id}.", ex);
            }
        }

        private static void ValidateShelter(Shelter shelter)
        {
            if (string.IsNullOrWhiteSpace(shelter.Name))
                throw new ArgumentException("O nome do abrigo é obrigatório.", nameof(shelter.Name));
            if (string.IsNullOrWhiteSpace(shelter.Address))
                throw new ArgumentException("O endereço do abrigo é obrigatório.", nameof(shelter.Address));
            if (shelter.Latitude < -90 || shelter.Latitude > 90)
                throw new ArgumentException("A latitude deve estar entre -90 e 90 graus.", nameof(shelter.Latitude));
            if (shelter.Longitude < -180 || shelter.Longitude > 180)
                throw new ArgumentException("A longitude deve estar entre -180 e 180 graus.", nameof(shelter.Longitude));
        }
    }
}