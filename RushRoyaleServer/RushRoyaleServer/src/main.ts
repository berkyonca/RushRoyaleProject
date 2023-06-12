const FindMatchRpc = "FindMatchRpc";

const LogicLoadedLoggerInfo = "Custom logic loaded.";
const MatchModuleName = "match";

function InitModule(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer)
{
    initializer.registerRpc(FindMatchRpc, findMatch);

    initializer.registerMatch(MatchModuleName, {
        matchInit,
        matchJoinAttempt,
        matchJoin,
        matchLeave,
        matchLoop,
        matchTerminate,
        matchSignal
    });

    logger.info(LogicLoadedLoggerInfo);
}
