using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using DataAccess;

namespace Business
{
    public class TagService : ITagService
    {
        private readonly SocialAppContext db;

        public TagService(SocialAppContext db)
        {
            this.db = db;
        }

        public Tag GetOrCreateTag(string name)
        {
            Tag existingTag = db.Tags.FirstOrDefault(t => t.Name == name);
            if (existingTag != null)
            {
                return existingTag;
            }
            Tag newTag = new Tag() { Name = name };
            return newTag;
        }

        public IEnumerable<Tag> GetOrCreateTags(IEnumerable<string> names)
        {
            return names.Select(GetOrCreateTag);
        }
    }
}
