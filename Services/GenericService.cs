using AutoMapper;
using Fakestagram.Data.DTOs.Pagination;
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
        protected readonly IGenericRepository<TModel> _repo;
        protected readonly IMapper _mapper;
        protected readonly IPaginationHelper _paginationHelper;

        public GenericService(IGenericRepository<TModel> repo, IMapper mapper, IPaginationHelper paginationHelper)
        {
            _repo = repo;
            _mapper = mapper;
            _paginationHelper = paginationHelper;
        }

        public virtual List<TReadDTO> GetAll(PaginationParameters @params)
        {
            int? skip = (@params?.Page - 1) * @params?.ItemsPerPage;
            int? take = @params?.ItemsPerPage;

            SetPaginationHeader(@params, take);

            return _repo.GetAll(skip, take).Select(entity => _mapper.Map<TModel, TReadDTO>((entity))).ToList();
        }

        protected void SetPaginationHeader(PaginationParameters @params, int? take, int totalCount = 0)
        {
            if (totalCount == 0)
            {
                totalCount = _repo.Count(null);
            }
            take ??= 0;

            if (take > 0)
            {
                _paginationHelper.SetPaginationHeader(totalCount, @params.Page, take.Value);
            }
            else
            {
                _paginationHelper.SetPaginationHeader(totalCount, 1, totalCount);
            }
        }

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

        public virtual TReadDTO Update(Guid entityId, TEditDTO updateDto)
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
