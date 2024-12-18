using Ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Ecommerce.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            
            var json = JsonConvert.SerializeObject(value);
            session.SetString(key, json);
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            
            var json = session.GetString(key);
            if (json == null)
                return default(T);

            
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static List<Cart> GetCartFromSession(this ISession session)
        {
            var cart = session.GetObjectFromJson<List<Cart>>("Cart");
            return cart ?? new List<Cart>(); 
        }

        
        public static void SetCartToSession(this ISession session, List<Cart> cart)
        {
            session.SetObjectAsJson("Cart", cart);
        }

    }
}
