﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZendeskApi.Client.Formatters;
using ZendeskApi.Client.Models;
using ZendeskApi.Client.Requests;
using ZendeskApi.Client.Responses;

namespace ZendeskApi.Client.Resources
{
    public class OrganizationsResource : IOrganizationsResource
    {
        private const string ResourceUri = "api/v2/organizations";

        private const string UserResourceUriFormat = "api/v2/users/{0}/organizations";

        private readonly IZendeskApiClient _apiClient;
        private readonly ILogger _logger;

        private Func<ILogger, string, IDisposable> _loggerScope =
            LoggerMessage.DefineScope<string>(typeof(OrganizationsResource).Name + ": {0}");

        public OrganizationsResource(IZendeskApiClient apiClient,
            ILogger logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IPagination<Organization>> GetAllAsync()
        {
            using (_loggerScope(_logger, "GetAllAsync"))
            using (var client = _apiClient.CreateClient())
            {
                var response = await client.GetAsync(ResourceUri).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<OrganizationsResponse>());
            }
        }

        public async Task<IPagination<Organization>> GetAllByUserIdAsync(long userId)
        {
            using (_loggerScope(_logger, $"GetAllAsync({userId})"))
            using (var client = _apiClient.CreateClient())
            {
                var response = await client.GetAsync(string.Format(UserResourceUriFormat, userId)).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Organization {0} not found", userId);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<OrganizationsResponse>());
            }
        }

        public async Task<Organization> GetAsync(long organizationId)
        {
            using (_loggerScope(_logger, $"GetAsync({organizationId})"))
            using (var client = _apiClient.CreateClient(ResourceUri))
            {
                var response = await client.GetAsync(organizationId.ToString()).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Organization {0} not found", organizationId);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<OrganizationResponse>()).Item;
            }
        }

        public async Task<IPagination<Organization>> GetAllAsync(long[] organizationIds)
        {
            using (_loggerScope(_logger, $"GetAllAsync({ZendeskFormatter.ToCsv(organizationIds)})"))
            using (var client = _apiClient.CreateClient(ResourceUri))
            {
                var response = await client.GetAsync($"show_many?ids={ZendeskFormatter.ToCsv(organizationIds)}").ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<OrganizationsResponse>());
            }
        }

        public async Task<IPagination<Organization>> GetAllByExternalIdsAsync(string[] externalIds)
        {
            using (_loggerScope(_logger, $"GetAllByExternalIdsAsync({ZendeskFormatter.ToCsv(externalIds)})"))
            using (var client = _apiClient.CreateClient(ResourceUri))
            {
                var response = await client.GetAsync($"show_many?external_ids={ZendeskFormatter.ToCsv(externalIds)}").ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<OrganizationsResponse>());
            }
        }

        public async Task<Organization> PostAsync(Organization organization)
        {
            using (_loggerScope(_logger, $"PostAsync"))
            using (var client = _apiClient.CreateClient())
            {
                var response = await client.PostAsJsonAsync(ResourceUri, new OrganizationRequest { Item = organization }).ConfigureAwait(false);

                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    throw new HttpRequestException(
                        $"Status code retrieved was {response.StatusCode} and not a 201 as expected" +
                        Environment.NewLine +
                        "See: https://developer.zendesk.com/rest_api/docs/core/organizations#create-organization");
                }

                return (await response.Content.ReadAsAsync<OrganizationResponse>()).Item;
            }
        }

        public async Task<Organization> PutAsync(Organization organization)
        {
            using (_loggerScope(_logger, $"PutAsync"))
            using (var client = _apiClient.CreateClient(ResourceUri))
            {
                var response = await client.PutAsJsonAsync(organization.Id.ToString(), new OrganizationRequest { Item = organization }).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Cannot update organization as organization {0} cannot be found", organization.Id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<OrganizationResponse>()).Item;
            }
        }

        public async Task DeleteAsync(long organizationId)
        {
            using (_loggerScope(_logger, $"DeleteAsync({organizationId})"))
            using (var client = _apiClient.CreateClient(ResourceUri))
            {
                var response = await client.DeleteAsync(organizationId.ToString());

                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new HttpRequestException(
                        $"Status code retrieved was {response.StatusCode} and not a 204 as expected" +
                        Environment.NewLine +
                        "See: https://developer.zendesk.com/rest_api/docs/core/users#delete-user");
                }
            }
        }
    }
}
