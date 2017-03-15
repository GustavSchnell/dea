﻿using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using Discord.WebSocket;
using System.Linq;

namespace DEA.Modules
{
    public class Crime : ModuleBase<SocketCommandContext>
    {

        [Command("Whore")]
        [Summary("Sell your body for some quick cash.")]
        [Remarks("Whore")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Whore()
        {
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                if (DateTime.Now.Subtract(await userRepo.GetLastWhore(Context.User.Id)).TotalMilliseconds > Config.WHORE_COOLDOWN)
                {
                    Random rand = new Random();
                    float moneyWhored = (float)(rand.Next((int)(Config.HIGHEST_WHORE) * 100)) / 100;
                    await userRepo.SetLastWhore(Context.User.Id, DateTime.Now);
                    await userRepo.EditCash(Context, moneyWhored);
                    await ReplyAsync($"{Context.User.Mention}, you whip it out and manage to rake in {moneyWhored.ToString("C2")}");
                }
                else
                {
                    var timeSpan = TimeSpan.FromMilliseconds(Config.WHORE_COOLDOWN - DateTime.Now.Subtract(await userRepo.GetLastWhore(Context.User.Id)).TotalMilliseconds);
                    var builder = new EmbedBuilder()
                    {
                        Title = $"Whore cooldown for {Context.User}",
                        Description = $"{timeSpan.Hours} Hours\n{timeSpan.Minutes} Minutes\n{timeSpan.Seconds} Seconds",
                        Color = new Color(49, 62, 255)
                    };
                    if (await guildRepo.GetDM(Context.Guild.Id))
                    {
                        var channel = await Context.User.CreateDMChannelAsync();
                        await channel.SendMessageAsync("", embed: builder);
                    } else
                        await ReplyAsync("", embed: builder);
                }
            }
        }

        [Command("Jump")]
        [Summary("Jump some random nigga in the hood.")]
        [Remarks("Jump")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Jump()
        {
            await RankHandler.RankRequired(Context, Ranks.Rank1);
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                if (DateTime.Now.Subtract(await userRepo.GetLastJump(Context.User.Id)).TotalMilliseconds > Config.JUMP_COOLDOWN)
                {
                    Random rand = new Random();
                    float moneyJumped = (float)(rand.Next((int)(Config.HIGHEST_JUMP) * 100)) / 100;
                    await userRepo.SetLastJump(Context.User.Id, DateTime.Now);
                    await userRepo.EditCash(Context, moneyJumped);
                    await ReplyAsync($"{Context.User.Mention}, you jump some random nigga on the streets and manage to get {moneyJumped.ToString("C2")}");
                }
                else
                {
                    var timeSpan = TimeSpan.FromMilliseconds(Config.JUMP_COOLDOWN - DateTime.Now.Subtract(await userRepo.GetLastJump(Context.User.Id)).TotalMilliseconds);
                    var builder = new EmbedBuilder()
                    {
                        Title = $"Jump cooldown for {Context.User}",
                        Description = $"{timeSpan.Hours} Hours\n{timeSpan.Minutes} Minutes\n{timeSpan.Seconds} Seconds",
                        Color = new Color(49, 62, 255)
                    };
                    if (await guildRepo.GetDM(Context.Guild.Id))
                    {
                        var channel = await Context.User.CreateDMChannelAsync();
                        await channel.SendMessageAsync("", embed: builder);
                    }
                    else
                        await ReplyAsync("", embed: builder);
                }
            }
        }

        [Command("Steal")]
        [Summary("Snipe some goodies from your local stores.")]
        [Remarks("Steal")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Steal()
        {
            await RankHandler.RankRequired(Context, Ranks.Rank2);
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                if (DateTime.Now.Subtract(await userRepo.GetLastSteal(Context.User.Id)).TotalMilliseconds > Config.STEAL_COOLDOWN)
                {
                    Random rand = new Random();
                    float moneySteal = (float)(rand.Next((int)(Config.HIGHEST_STEAL) * 100)) / 100;
                    await userRepo.SetLastSteal(Context.User.Id, DateTime.Now);
                    await userRepo.EditCash(Context, moneySteal);
                    string randomStore = Config.STORES[rand.Next(1, Config.STORES.Length) - 1];
                    await ReplyAsync($"{Context.User.Mention}, you walk in to your local {randomStore}, point a fake gun at the clerk, and manage to walk away " +
                                     $"with {moneySteal.ToString("C2")}");
                }
                else
                {
                    var timeSpan = TimeSpan.FromMilliseconds(Config.STEAL_COOLDOWN - DateTime.Now.Subtract(await userRepo.GetLastSteal(Context.User.Id)).TotalMilliseconds);
                    var builder = new EmbedBuilder()
                    {
                        Title = $"Steal cooldown for {Context.User}",
                        Description = $"{timeSpan.Hours} Hours\n{timeSpan.Minutes} Minutes\n{timeSpan.Seconds} Seconds",
                        Color = new Color(49, 62, 255)
                    };
                    if (await guildRepo.GetDM(Context.Guild.Id))
                    {
                        var channel = await Context.User.CreateDMChannelAsync();
                        await channel.SendMessageAsync("", embed: builder);
                    }
                    else
                        await ReplyAsync("", embed: builder);
                }
            }
        }

        [Command("Rob")]
        [Summary("Lead a large scale operation on a local bank.")]
        [Remarks("Rob <Amount of cash to spend on resources>")]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Rob(float resources)
        {
            await RankHandler.RankRequired(Context, Ranks.Rank4);
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                if (DateTime.Now.Subtract(await userRepo.GetLastRob(Context.User.Id)).TotalMilliseconds > Config.ROB_COOLDOWN)
                {
                    if (await userRepo.GetCash(Context.User.Id) < resources) throw new Exception($"You do not have enough money. Current cash: {(await userRepo.GetCash(Context.User.Id)).ToString("C2")}");
                    if (resources < Config.MIN_RESOURCES) throw new Exception($"The minimum amount of money to spend on resources for rob is {Config.MIN_RESOURCES.ToString("C2")}.");
                    if (resources > Config.MAX_RESOURCES) throw new Exception($"The maximum amount of money to spend on resources for rob is {Config.MAX_RESOURCES.ToString("C2")}.");
                    Random rand = new Random();
                    float odds = (rand.Next(5000, 7500)) / 100.00f;
                    float multiplier = 3 + ((75 - odds) / 16.6666666666f);
                    float moneyStolen = resources * multiplier;
                    await userRepo.SetLastRob(Context.User.Id, DateTime.Now);
                    string randomBank = Config.BANKS[rand.Next(1, Config.BANKS.Length) - 1];

                    float success = (rand.Next(10000)) / 100;
                    if (success >= odds)
                    {
                        await userRepo.EditCash(Context, moneyStolen);
                        await ReplyAsync($"{Context.User.Mention}, with a {odds.ToString("N2")}% chance of success, you successfully stole " +
                        $"{moneyStolen.ToString("C2")} from the {randomBank}. Current balance: {(await userRepo.GetCash(Context.User.Id)).ToString("C2")}$.");
                    }
                    else
                    {
                        await userRepo.EditCash(Context, -resources);
                        await ReplyAsync($"{Context.User.Mention}, with a {odds.ToString("N2")}% chance of success, you failed to steal " +
                        $"{moneyStolen.ToString("C2")} from the {randomBank}. Current balance: {(await userRepo.GetCash(Context.User.Id)).ToString("C2")}.");
                    }
                }
                else
                {
                    var timeSpan = TimeSpan.FromMilliseconds(Config.ROB_COOLDOWN - DateTime.Now.Subtract(await userRepo.GetLastRob(Context.User.Id)).TotalMilliseconds);
                    var builder = new EmbedBuilder()
                    {
                        Title = $"{await guildRepo.GetPrefix(Context.Guild.Id)}Rob cooldown for {Context.User}",
                        Description = $"{timeSpan.Hours} Hours\n{timeSpan.Minutes} Minutes\n{timeSpan.Seconds} Seconds",
                        Color = new Color(49, 62, 255)
                    };
                    if (await guildRepo.GetDM(Context.Guild.Id))
                    {
                        var channel = await Context.User.CreateDMChannelAsync();
                        await channel.SendMessageAsync("", embed: builder);
                    }
                    else
                        await ReplyAsync("", embed: builder);
                }
            }
        }

        [Command("Bully")]
        [Summary("Bully anyone's nickname to whatever you please.")]
        [Remarks("Bully <@User> <Nickname>")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task Bully(SocketGuildUser userToBully, [Remainder] string nickname)
        {
            await RankHandler.RankRequired(Context, Ranks.Rank3);
            if (nickname.Length > 32) throw new Exception("The length of a nickname can be a maximum of 32 characters.");
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var role3 = Context.Guild.GetRole(await guildRepo.GetRank3Id(Context.Guild.Id));
                if (role3.Position <= userToBully.Roles.OrderByDescending(x => x.Position).First().Position)
                    throw new Exception($"You cannot bully someone with role higher or equal to: {role3.Mention}");
                await userToBully.ModifyAsync(x => x.Nickname = nickname);
                await ReplyAsync($"{userToBully.Mention} just got ***BULLIED*** by {Context.User.Mention} with his new nickname: \"{nickname}\".");
            }
        }
    }
}
