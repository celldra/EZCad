using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Events;

public class PlayerConnectingEvent : BaseApiEvent
{
    [EventHandler("playerConnecting")]
    public async void Handle([FromSource] Player player, string playerName, dynamic setKickReason,
        dynamic deferrals)
    {
        try
        {
            deferrals.defer();

            await Delay(0);

            deferrals.update("We're checking if you're in our CAD, this should only take a moment...");
            
            var licenseId = player.Identifiers["license"];

            Debug.WriteLine($"New player joined ({playerName}) with license ID: {licenseId}");

            var response = await Api.LoginAsync(playerName, licenseId);

            if (response?.Success != true) throw new ApiException("Not found in CAD");
            
            Debug.WriteLine("Adding principals");

            var permissionAdd = $"add_principal identifier.license:{licenseId} group.";

            foreach (var role in response.Profile.Roles)
            {
                var newRole = role.Replace("Administrator", "admin");
                newRole = newRole.ToLower();

                var command = $"{permissionAdd}{newRole}";

                await Delay(0);
                API.ExecuteCommand(command);
                Debug.WriteLine($"Added to role group {newRole}");
            }

            MemoryStorage.AuthorizedPlayers.TryAdd(licenseId, response.Profile.Roles);
            MemoryStorage.AuthorizedIdentities.TryAdd(licenseId, response.Identity);

            await Delay(0);

            deferrals.update($"Welcome back, {response.Identity.FirstName}!");

            await Delay(500);

            deferrals.done();
        }
        catch (BannedException ex)
        {
            await Delay(0);
            if (ex.BanResponse.IsPermanent)
            {
                deferrals.done(
                    $"You've been permanently banned from the server for {ex.BanResponse.Reason} by {ex.BanResponse.BannedBy}\n\nFor appeals, you need to contact a support member");
                return;
            }

            deferrals.done(
                $"You've been banned from the server for {ex.BanResponse.Reason} by {ex.BanResponse.BannedBy}\nYour ban will expire at: {ex.BanResponse.Expiration}\n\nFor appeals, you need to contact a support member");
        }
        catch (ApiException)
        {
            var card =
                $@"{{""type"":""AdaptiveCard"",""$schema"":""https://adaptivecards.io/schemas/adaptive-card.json"",""version"":""1.2"",""body"":[{{""type"":""Container"",""items"":[{{""type"":""TextBlock"",""text"":""{MemoryStorage.Configuration.ServerName.Replace("\"", "\\\"")}"",""wrap"":true,""fontType"":""Default"",""size"":""ExtraLarge"",""weight"":""Bolder"",""color"":""Light""}},{{""type"":""TextBlock"",""text"":""{MemoryStorage.Configuration.CadNotLinkedCardText.Replace("\"", "\\\"")}"",""wrap"":true,""size"":""Large"",""weight"":""Bolder"",""color"":""Light""}},{{""type"":""TextBlock"",""text"":""For the full experience, you need to setup your identity in our CAD, upon joining you'll be given 5 minutes to do so or you'll be kicked automatically!"",""wrap"":true,""color"":""Light"",""size"":""Medium""}},{{""type"":""ColumnSet"",""height"":""stretch"",""minHeight"":""100px"",""bleed"":true,""horizontalAlignment"":""Center"",""selectAction"":{{""type"":""Action.OpenUrl""}},""columns"":[{{""type"":""Column"",""width"":""stretch"",""items"":[{{""type"":""ActionSet"",""actions"":[{{""type"":""Action.OpenUrl"",""title"":""CAD"",""url"":""{MemoryStorage.Configuration.CadUrl.Replace("\"", "\\\"")}"",""style"":""positive""}}]}}]}},{{""type"":""Column"",""width"":""stretch"",""items"":[{{""type"":""ActionSet"",""actions"":[{{""type"":""Action.Submit"",""title"":""Play"",""style"":""positive"",""id"":""played""}}]}}]}},{{""type"":""Column"",""width"":""stretch"",""items"":[{{""type"":""ActionSet"",""actions"":[{{""type"":""Action.OpenUrl"",""title"":""Discord"",""style"":""positive"",""url"":""{MemoryStorage.Configuration.DiscordUrl.Replace("\"", "\\\"")}""}}]}}]}}]}}],""style"":""default"",""bleed"":true,""height"":""stretch"",""isVisible"":true}}]}}";
            
            Debug.WriteLine("Presenting not in CAD card");

            await Delay(0);

            deferrals.update(
                $"You weren't detected in our CAD! You can create an account for it at {MemoryStorage.Configuration.CadUrl}. Either way, you're still being connected to {MemoryStorage.Configuration.ServerName}");

            await Delay(5000);

            deferrals.done();
        }
        finally
        {
            Api.Dispose();
        }
    }
}