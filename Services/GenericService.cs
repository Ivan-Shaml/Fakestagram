using AutoMapper;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;

namespace Fakestagram.Services
{
    public class GenericService<TModel, TCreateDTO, TEditDTO, TReadDTO> : IGenericService<TModel, TCreateDTO, TEditDTO, TReadDTO>
        where TModel : BaseModel
        where TCreateDTO : class
        where TEditDTO : class
        where TReadDTO : class
    {
        protected readonly IPostsRepository<TModel> _repo;
        protected readonly IMapper _mapper;

        public GenericService(IPostsRepository<TModel> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public virtual List<TReadDTO> GetAll() => _repo.GetAll().Select(entity => _mapper.Map<TModel, TReadDTO>((entity))).ToList();

        public virtual TReadDTO GetById(Guid id)
        {
            TModel entity = _repo.GetById(id);

            if (entity is null)
            {
                return null;
            }

            return _mapper.Map<TModel, TReadDTO>(entity);
        }

        public virtual TReadDTO Insert(TCreateDTO createDto)
        {
            TModel entity = _mapper.Map<TCreateDTO, TModel>(createDto);
            
            _repo.Create(entity);
            
            return _mapper.Map<TModel, TReadDTO>(entity);
        }

        public virtual TReadDTO Update(TEditDTO updateDto)
        {
            TModel entity = _mapper.Map<TEditDTO, TModel>(updateDto);

            _repo.Update(entity);

            return _mapper.Map<TModel, TReadDTO>(entity);
        }

        public virtual void Delete(Guid id)
        {
            _repo.Delete(id);
        }

    }
}
