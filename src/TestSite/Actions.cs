using System;
using System.Collections;
//using WebOnDiet.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebOnDiet;
using WebOnDiet.Framework;

namespace TestSite
{
	public class Actions
	{
		readonly IPostsRepository _postsRepository;

		public Actions(IPostsRepository postsRepository)
		{
			_postsRepository = postsRepository;
		}

		[Get("/")]
		public string Testing()
		{
			return "Hello";
		}

		[Get("/simpleView")]
		public void SimpleView()
		{
			wod.Render.Template("/simple", new Hashtable { { "a", 3 } });
		}

		[Get("/posts")]
		public string Posts()
		{
			return _postsRepository.GetRecent().Select(x=>x.ToString()).Aggregate((x,y)=>x+"<br/>"+y);
		}
	}

	public interface IPostsRepository
	{
		IQueryable<Post> GetRecent();
	}
	public class PostsRepository:IPostsRepository
	{
		private List<Post> posts = new List<Post>
		                           	{
		                           		new Post {Id = 1, Title = "post no 1"},
		                           		new Post {Id = 2, Title = "post no 2"},
		                           		new Post {Id = 3, Title = "post no 3"}
		                           	};
		public IQueryable<Post> GetRecent()
		{
			return posts.AsQueryable();
		}
	}
	public class Post
	{
		public int Id;
		public string Title;

		public override string ToString()
		{
			return string.Format("post#{0}[{1}]",Id, Title);
		}
	}
}