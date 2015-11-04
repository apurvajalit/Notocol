using Mixpanel.NET.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Business
{
    public class ActivityTracker
    {
        MixpanelTracker tracker;
        private string token = "0a91945b241bcaefa7ebbbb8277e9fe1";
        //var tracker;
        public ActivityTracker(){
            tracker = new MixpanelTracker(token);
        }

        public void TrackEvent(string eventName, string userName, Dictionary<string, object> properties, string distintID = null)
        {
            if (distintID != null)
            {
                properties.Add("distinct_id", distintID);
            }
            properties.Add("user", userName);
            Task.Factory.StartNew(() =>
               tracker.Track(eventName, properties));
            
        }
    }
}
