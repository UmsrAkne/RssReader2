using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// NgWordをリポジトリに追加します。
        /// </summary>
        /// <param name="ngWord">追加したいNgWordオブジェクト。</param>
        /// <remarks>
        /// 重複する単語がすでに存在する場合は、そのNgWordは追加されません。
        /// また、追加時にLastUpdatedが現在の日時に設定されます。
        /// </remarks>
        public void AddNgWord(NgWord ngWord)
        {
            var all = GetAllNgWords();
            if (all.Any(w => w.Word == ngWord.Word))
            {
                return;
            }

            ngWord.LastUpdated = DateTime.Now;
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

            ngWord.IsDeleted = true;
            UpdateNgWord(ngWord);
        }
    }
}