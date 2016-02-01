﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Drum
{
    public static class HttpConfigurationExtensions
    {
        private static readonly object ContextKey = new object();

        public static UriMakerContext MapHttpAttributeRoutesAndUseUriMaker(
            this HttpConfiguration configuration,
            IDirectRouteProvider directRouteProvider = null,
            Func<HttpRequestMessage, ICollection<RouteEntry>, RouteEntry> routeSelector = null)
        {
            directRouteProvider = directRouteProvider ?? new DefaultDirectRouteProvider();
            var decorator = new DecoratorRouteProvider(directRouteProvider, routeSelector);
            configuration.MapHttpAttributeRoutes(decorator);
            var uriMakerContext = new UriMakerContext(decorator.RouteMap);
            configuration.Properties.AddOrUpdate(ContextKey, _ => uriMakerContext, (_, __) => uriMakerContext);
            return uriMakerContext;
        }

        public static UriMakerContext MapHttpAttributeRoutesAndUseUriMaker(
            this HttpConfiguration configuration,           
            IInlineConstraintResolver constraintResolver,
            IDirectRouteProvider directRouteProvider = null,
            Func<HttpRequestMessage, ICollection<RouteEntry>, RouteEntry> routeSelector = null)
        {
            directRouteProvider = directRouteProvider ?? new DefaultDirectRouteProvider();
            var decorator = new DecoratorRouteProvider(directRouteProvider, routeSelector);
            configuration.MapHttpAttributeRoutes(constraintResolver, decorator);
            var uriMakerContext = new UriMakerContext(decorator.RouteMap);
            configuration.Properties.AddOrUpdate(ContextKey, _ => uriMakerContext, (_, __) => uriMakerContext);
            return uriMakerContext;
        }

        internal static UriMakerContext TryGetUriMakerContext(this HttpConfiguration config)
        {
            object contextObject;
            config.Properties.TryGetValue(ContextKey, out contextObject);
            return contextObject as UriMakerContext;
        }
    }
}