using System;
using System.Collections.Generic;

namespace RssReader2.Models.Dbs
{
    public class NgWordService
    {
        private readonly IRepository<NgWord> ngWordRepository;

        public NgWordService(IRepository<NgWord> ngWordRepository)
        {
            this.ngWordRepository = ngWordRepository;
        }

        public IEnumerable<NgWord> GetAllNgWords()
        {
            return ngWordRepository.GetAll();
        }

        public NgWord GetNgWordById(int id)
        {
            return ngWordRepository.GetById(id);
        }

        public void AddNgWord(NgWord ngWord)
        {
            ngWordRepository.Add(ngWord);
        }

        public void AddNgWords(IEnumerable<NgWord> ngWords)
        {
            ngWordRepository.AddRange(ngWords);
        }

        public void UpdateNgWord(NgWord ngWord)
        {
            ngWordRepository.Update(ngWord);
        }

        public void DeleteNgWord(int id)
        {
            var ngWord = ngWordRepository.GetById(id);
            if (ngWord == null)
            {
                throw new ArgumentException("not found.");
            }

            ngWordRepository.Delete(id);
        }
    }
}