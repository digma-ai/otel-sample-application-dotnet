using Sample.MoneyTransfer.API;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

RunWebApp.Run(args);
