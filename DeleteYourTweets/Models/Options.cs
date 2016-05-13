using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteYourTweets.Models
{
    public class Options
    {
        [DisplayName("Tüm Tweetleri Sil")]
        public bool Tweets { get; set; }
        
        [DisplayName("Tüm Favorileri Sil")]
        public bool Favourites { get; set; }

        [DisplayName("Tüm Mesajları Sil (Beta)")]
        public bool Messages { get; set; }

        [DisplayName("Tüm Takip ettiklerini Sil")]
        public bool Followings { get; set; }
    }
}
