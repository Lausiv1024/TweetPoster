using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweeter
{
    public class TwitterAPIAuthData
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }

        public override string ToString()
        {
            return $"CKey : {ConsumerKey}\nCSecret :  {ConsumerSecret}\nAToken : {AccessToken}\nASecret : {AccessTokenSecret}";
        }
    }
}
