using Fakestagram.Models;

namespace Fakestagram.Services.Contracts
{
    public interface IGenericService<TModel, TCreateDTO, TEditDTO, TReadDTO>
        where TModel : BaseModel
        where TCreateDTO : class
        where TEditDTO : class
        where TReadDTO : class
    {
        List<TReadDTO> GetAll();
        TReadDTO GetById(Guid id);
        TReadDTO Insert(TCreateDTO createDto);
        TReadDTO Update(TEditDTO updateDto);
        void Delete(Guid id);
    }
}
