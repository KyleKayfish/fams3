﻿using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchRequestAdaptor.Configuration;
using SearchRequestAdaptor.Publisher;
using SearchRequestAdaptor.Publisher.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Notifier
{
    public interface ISearchRequestNotifier<T>
    {
        Task NotifySearchRequestEventAsync(string searchRequestKey, T searchRequestOrdered, CancellationToken cancellationToken);
    }

    public class WebHookSearchRequestNotifier : ISearchRequestNotifier<SearchRequestOrdered>
    {

        private readonly HttpClient _httpClient;
        private readonly SearchRequestAdaptorOptions _searchRequestOptions;
        private readonly ILogger<WebHookSearchRequestNotifier> _logger;
        private readonly ISearchRequestEventPublisher _searchRequestEventPublisher;


        public WebHookSearchRequestNotifier(HttpClient httpClient, IOptions<SearchRequestAdaptorOptions> searchRequestOptions, ILogger<WebHookSearchRequestNotifier> logger, ISearchRequestEventPublisher searchRequestEventPublisher)
        {
            _httpClient = httpClient;
            _logger = logger;
            _searchRequestOptions = searchRequestOptions.Value;
            _searchRequestEventPublisher = searchRequestEventPublisher;
        }

        public async Task NotifySearchRequestEventAsync(string searchRequestKey, SearchRequestOrdered searchRequestOrdered,
           CancellationToken cancellationToken)
        {
            var webHookName = "SearchRequest";
            string eventName = "SearchRequestOrdered";

            if (searchRequestOrdered == null) throw new ArgumentNullException(nameof(SearchRequestOrdered));

            foreach (var webHook in _searchRequestOptions.WebHooks)
            {
                _logger.LogDebug(
                   $"The webHook {webHookName} notification is attempting to send status {eventName} event for {webHook.Name} webhook.");

                if (!URLHelper.TryCreateUri(webHook.Uri, eventName, $"{searchRequestKey}", out var endpoint))
                {
                    _logger.LogWarning(
                        $"The webHook {webHookName} notification uri is not established or is not an absolute Uri for {webHook.Name}. Set the WebHook.Uri value on SearchApi.WebHooks settings.");
                    await _searchRequestEventPublisher.PublishSearchRequestFailed(
                       searchRequestOrdered, "notification uri is not established or is not an absolute Uri."
                        );
                    return;
                }

                using var request = new HttpRequestMessage();

                try
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(searchRequestOrdered));

                    content.Headers.ContentType =
                        System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request.Content = content;
                    request.Method = HttpMethod.Post;
                    request.Headers.Accept.Add(
                        System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    request.RequestUri = endpoint;
                    var response = await _httpClient.SendAsync(request, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError(
                            $"The webHook {webHookName} notification has not executed status {eventName} successfully for {webHook.Name} webHook. The error code is {response.StatusCode.GetHashCode()}.");
                        await _searchRequestEventPublisher.PublishSearchRequestRejected(
                                searchRequestOrdered,
                                new List<ValidationResult>()
                                {
                                    new ValidationResultData(){ PropertyName=response.StatusCode.ToString(), ErrorMessage=await response.Content.ReadAsStringAsync()}
                                });
                        return;
                    }

                    _logger.LogInformation(
                        $"The webHook {webHookName} notification has executed status {eventName} successfully for {webHook.Name} webHook.");
                    await _searchRequestEventPublisher.PublishSearchRequestSubmitted(searchRequestOrdered, "Search Request has been submitted successfully.");
                }
                catch (Exception exception)
                {
                    await _searchRequestEventPublisher.PublishSearchRequestFailed(searchRequestOrdered, exception.Message);
                    _logger.LogError($"The webHook {webHookName} notification failed for status {eventName} for {webHook.Name} webHook. [{exception.Message}]");
                }
            }
        }
    }
}