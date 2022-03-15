﻿using AutoMapper;
using Fakestagram.Data.DTOs;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;

namespace Fakestagram.Services
{
    public class LikesService : ILikesService
    {
        private readonly ICommentLikesRepository _commentLikesRepo;
        private readonly IPostLikesRepository _postLikesRepo;
        private readonly IUserService _userService;
        private readonly IPostsRepository _postsRepository;
        private readonly ICommentsRepository _commentsRepository;
        private readonly IMapper _mapper;

        public LikesService(ICommentLikesRepository commentLikesRepo, IPostLikesRepository postLikesRepo,
                            IUserService userService, IPostsRepository postsRepository,
                            ICommentsRepository commentsRepository, IMapper mapper
                            )
        {
            _commentLikesRepo = commentLikesRepo;
            _postLikesRepo = postLikesRepo;
            _userService = userService;
            _postsRepository = postsRepository;
            _commentsRepository = commentsRepository;
            _mapper = mapper;
        }
        public void DislikeComment(Guid targetCommentId)
        {
            User currentUser = _userService.GetCurrentUser();

            var commentLike = _commentLikesRepo.GetByCommentId(targetCommentId, currentUser.Id) ?? throw new EntityAlreadyDislikedException("The Comment is already disliked.");

            _commentLikesRepo.Delete(commentLike.Id);
        }

        public void DislikePost(Guid targetPostId)
        {
            User currentUser = _userService.GetCurrentUser();

            var postLike = _postLikesRepo.GetByPostId(targetPostId, currentUser.Id) ?? throw new EntityAlreadyDislikedException("The Post is already disliked.");

            _postLikesRepo.Delete(postLike.Id);
        }

        public List<UserListLikesDTO> GetUsersLikedCommentList(Guid targetCommentId)
        {
            var usersLikedList = _commentLikesRepo.GetUserListLikes(targetCommentId);

            return _mapper.Map<List<UserListLikesDTO>>(usersLikedList);
        }

        public List<UserListLikesDTO> GetUsersLikedPostList(Guid targetPostId)
        {
            var usersLikedList = _postLikesRepo.GetUserListLikes(targetPostId);

            return _mapper.Map<List<UserListLikesDTO>>(usersLikedList);
        }

        public void LikeComment(Guid targetCommentId)
        {
            User currentUser = _userService.GetCurrentUser();

            var checkEntity = _commentLikesRepo.GetByCommentId(targetCommentId, currentUser.Id);

            if (checkEntity is not null)
            {
                throw new EntityAlreadyLikedException("You have already liked this comment.");
            }

            _commentsRepository.GetById(targetCommentId);

            var commentLike = new CommentLike()
            {
                CommentId = targetCommentId,
                UserId = currentUser.Id
            };

            _commentLikesRepo.Create(commentLike);
        }

        public void LikePost(Guid targetPostId)
        {
            User currentUser = _userService.GetCurrentUser();

            var checkEntity = _postLikesRepo.GetByPostId(targetPostId, currentUser.Id);

            if (checkEntity is not null)
            {
                throw new EntityAlreadyLikedException("You have already liked this post.");
            }

            _postsRepository.GetById(targetPostId);

            var postLike = new PostLike()
            {
                PostId = targetPostId,
                UserId = currentUser.Id
            };

            _postLikesRepo.Create(postLike);
        }
    }
}
