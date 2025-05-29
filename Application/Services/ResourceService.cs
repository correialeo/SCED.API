using SCED.API.Application.Interfaces;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.Domain.Interfaces;

namespace SCED.API.Application.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResourceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
        {
            try
            {
                return await _unitOfWork.Resources.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar todos os recursos.", ex);
            }
        }

        public async Task<Resource?> GetResourceByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do recurso deve ser maior que zero.", nameof(id));

            try
            {
                return await _unitOfWork.Resources.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar recurso com ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Resource>> GetResourcesByTypeAsync(ResourceType type)
        {
            if (!Enum.IsDefined(typeof(ResourceType), type))
                throw new ArgumentException("Tipo de recurso inválido.", nameof(type));

            try
            {
                return await _unitOfWork.Resources.GetResourcesByTypeAsync(type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar recursos do tipo {type}.", ex);
            }
        }

        public async Task<IEnumerable<Resource>> GetAvailableResourcesAsync()
        {
            try
            {
                return await _unitOfWork.Resources.GetAvailableResourcesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar recursos disponíveis.", ex);
            }
        }

        public async Task<IEnumerable<Resource>> GetResourcesByStatusAsync(ResourceStatus status)
        {
            if (!Enum.IsDefined(typeof(ResourceStatus), status))
                throw new ArgumentException("Status do recurso inválido.", nameof(status));

            try
            {
                return await _unitOfWork.Resources.GetResourcesByStatusAsync(status);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar recursos com status {status}.", ex);
            }
        }

        public async Task<IEnumerable<Resource>> GetNearbyResourcesAsync(double latitude, double longitude, double radiusKm = 5.0)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("A latitude deve estar entre -90 e 90 graus.", nameof(latitude));
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("A longitude deve estar entre -180 e 180 graus.", nameof(longitude));
            if (radiusKm <= 0 || radiusKm > 1000)
                throw new ArgumentException("O raio deve estar entre 0 e 1000 km.", nameof(radiusKm));

            try
            {
                return await _unitOfWork.Resources.GetResourcesInRadiusAsync(latitude, longitude, radiusKm);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar recursos na área especificada.", ex);
            }
        }

        public async Task<Resource> CreateResourceAsync(Resource resource)
        {
            if (resource == null)
                throw new ArgumentNullException(nameof(resource), "O recurso não pode ser nulo.");

            ValidateResource(resource);

            try
            {

                await _unitOfWork.Resources.AddAsync(resource);
                await _unitOfWork.SaveChangesAsync();
                return resource;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao criar o recurso.", ex);
            }
        }

        public async Task<Resource?> UpdateResourceAsync(long id, Resource updatedResource)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do recurso deve ser maior que zero.", nameof(id));
            if (updatedResource == null)
                throw new ArgumentNullException(nameof(updatedResource), "O recurso atualizado não pode ser nulo.");
            if (id != updatedResource.Id)
                throw new ArgumentException("O ID fornecido não corresponde ao ID do recurso.", nameof(id));

            ValidateResource(updatedResource);

            try
            {
                Resource existingResource = await _unitOfWork.Resources.GetByIdAsync(id);
                if (existingResource == null)
                    return null;

                existingResource.Type = updatedResource.Type;
                existingResource.Quantity = updatedResource.Quantity;
                existingResource.Latitude = updatedResource.Latitude;
                existingResource.Longitude = updatedResource.Longitude;
                existingResource.Status = updatedResource.Status;

                await _unitOfWork.Resources.UpdateAsync(existingResource);
                await _unitOfWork.SaveChangesAsync();
                return existingResource;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar o recurso com ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteResourceAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do recurso deve ser maior que zero.", nameof(id));

            try
            {
                Resource resource = await _unitOfWork.Resources.GetByIdAsync(id);
                if (resource == null)
                    return false;

                await _unitOfWork.Resources.DeleteAsync(resource);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao deletar o recurso com ID {id}.", ex);
            }
        }

        private static void ValidateResource(Resource resource)
        {
            if (!Enum.IsDefined(typeof(ResourceType), resource.Type))
                throw new ArgumentException("Tipo de recurso inválido.", nameof(resource.Type));
            if (resource.Quantity < 0)
                throw new ArgumentException("A quantidade deve ser maior ou igual a zero.", nameof(resource.Quantity));
            if (resource.Latitude < -90 || resource.Latitude > 90)
                throw new ArgumentException("A latitude deve estar entre -90 e 90 graus.", nameof(resource.Latitude));
            if (resource.Longitude < -180 || resource.Longitude > 180)
                throw new ArgumentException("A longitude deve estar entre -180 e 180 graus.", nameof(resource.Longitude));
            if (!Enum.IsDefined(typeof(ResourceStatus), resource.Status))
                throw new ArgumentException("Status do recurso inválido.", nameof(resource.Status));
        }
    }
}