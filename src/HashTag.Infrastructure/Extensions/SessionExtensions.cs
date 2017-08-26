using System.Collections.Generic;
using HashTag.Infrastructure.Alerts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HashTag.Infrastructure.Extensions
{
    public static class SessionExtensions
    {
        private const string AlertsKey = "_Alerts";

        private static T Get<T>(this ISession session, string key)
        {
            var json = session.GetString(key);

            return string.IsNullOrEmpty(json)
                ? default(T)
                : JsonConvert.DeserializeObject<T>(json);
        }

        private static T GetWithRemove<T>(this ISession session, string key)
        {
            var result = session.Get<T>(key);
            session.Remove(key);

            return result;
        }

        private static void Set(this ISession session, string key, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            session.SetString(key, json);
        }

        public static List<Alert> GetAlerts(this ISession session)
        {
            var alerts = session.Get<List<Alert>>(AlertsKey) ?? new List<Alert>();

            return alerts;
        }

        public static List<Alert> GetAlertsWithRemove(this ISession session)
        {
            var alerts = session.GetWithRemove<List<Alert>>(AlertsKey) ?? new List<Alert>();

            return alerts;
        }

        public static void AddAlert(this ISession session, Alert alert)
        {
            var alerts = session.Get<List<Alert>>(AlertsKey) ?? new List<Alert>();
            alerts.Add(alert);
            session.Set(AlertsKey, alerts);
        }
    }
}