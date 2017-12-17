using System;
using System.Linq;
using System.Data.Entity;

namespace Vidzy
{
    class Program
    {
        static void Main(string[] args)
        {
            //Uncomment the method calls to run the methods.

            //var video1 = new Video { Name = "Terminator", GenreId = 2, ReleaseDate = new DateTime(1984, 8, 26), Classification = Classification.Silver };
            //addVideo(video1);

            //addTags("classics", "drama");

            //addTagsToVideo(1, "classics", "drama", "comedy");

            //removeTagsFromVideo(1, "comedy");

            //removeVideo(1);

            //removeGenre(2, true);
        }

        public static void addVideo(Video video)
        {
            using(var context = new VidzyContext())
            {
                context.Videos.Add(video);
                context.SaveChanges();
            }
        }

        public static void addTags(params string[] tagNames)
        {
            using(var context = new VidzyContext())
            {
                var tags = context.Tags.Where(t => tagNames.Contains(t.Name)).ToList();


                foreach(var name in tagNames)
                {
                    if(!tags.Any(t => t.Name.Equals(name))){
                        context.Tags.Add(new Tag { Name = name });
                    }

                }
                context.SaveChanges();
            }
        }

        public static void addTagsToVideo(int videoId, params string[] tagNames)
        {
            using (var context = new VidzyContext())
            {
                var tags = context.Tags.Where(t => tagNames.Contains(t.Name)).ToList();
                var video = context.Videos.Find(videoId);
                var videoTags = video.Tags;

                foreach (var name in tagNames)
                {
                    if (!tags.Any(t => t.Name.Equals(name)))
                    {
                        tags.Add(new Tag { Name = name });
                    }
                    if(!videoTags.Any(t => t.Name.Equals(name)))
                    {
                        video.Tags.Add(new Tag { Name = name });
                    }

                }

                context.SaveChanges();

            }
        }

        public static void removeTagsFromVideo(int videoId, params string[] tagNames)
        {
            using (var context = new VidzyContext())
            {
                context.Tags.Where(t => tagNames.Contains(t.Name)).Load();

                var video = context.Videos.Find(videoId);

                foreach(var name in tagNames)
                {
                    video.RemoveTag(name);
                }

                context.SaveChanges();
            }
        }

        public static void removeVideo(int videoId)
        {
            using (var context = new VidzyContext())
            {
                var video = context.Videos.Find(videoId);
                if (video == null) return;

                context.Videos.Remove(video);
                context.SaveChanges();
            }
        }

        public static void removeGenre(int genreId, bool enforceDeletingVideos)
        {
            using (var context = new VidzyContext())
            {
                var genre = context.Genres.Include(g => g.Videos).SingleOrDefault(g => g.Id == genreId);
                if (genre == null) return;

                if (enforceDeletingVideos)
                {
                    context.Videos.RemoveRange(genre.Videos);
                }

                context.Genres.Remove(genre);
                context.SaveChanges();
            }
        }
    }
}
