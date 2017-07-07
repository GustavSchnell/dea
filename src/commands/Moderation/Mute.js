const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');
const config = require('../../config.json');
const ModerationService = require('../../services/ModerationService.js');

class Mute extends patron.Command {
  constructor() {
    super({
      name: 'mute',
      group: 'moderation',
      description: 'Mute any user.',
      args: [
        new patron.Argument({
          name: 'member',
          key: 'member',
          type: 'member',
          example: '"Billy Steve#0711"'
        }),
        new patron.Argument({
          name: 'number of hours',
          key: 'hours',
          type: 'float',
          example: '48',
          default: 24
        }),
        new patron.Argument({
          name: 'reason',
          key: 'reason',
          type: 'string',
          default: '',
          example: 'was spamming like a chimney',
          remainder: true
        })
      ]
    });
  }

  async run(context, args) {
    const dbGuild = await db.guildRepo.getGuild(context.guild.id);

    if (dbGuild.roles.muted === null) {
      return util.Messenger.replyError(context.channel, context.author, 'You must set a muted role with the `' + config.prefix + 'setmute @Role` command before you can mute users.');
    } else if (args.member.roles.has(dbGuild.roles.muted)) {
      return util.Messenger.replyError(context.channel, context.author, 'This user is already muted.');
    }

    const role = context.guild.roles.get(dbGuild.roles.muted);

    if (role === undefined) {
      return util.Messenger.replyError(context.channel, context.author, 'The set muted role has been deleted. Please set a new one with the `' + config.prefix + 'setmute @Role` command.');
    }
    
    await args.member.addRole(role);
    await db.muteRepo.insertMute(args.member.id, context.guild.id, util.NumberUtil.hoursToMs(args.hours));
    await util.Messenger.reply(context.channel, context.author, 'You have successfully muted ' + args.member.user.tag + ' for ' + Math.round(args.hours) + ' hour' + (args.hours === 1 ? '' : 's') + '.');
    await ModerationService.tryInformUser(context.guild, context.author, 'mute', args.member.user, args.reason);
    await ModerationService.tryMogLog(dbGuild, context.guild, 'Mute', config.muteColor, args.reason, context.author, args.member.user);
  }
}

module.exports = new Mute();