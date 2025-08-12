using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private readonly IConfigurationManager _configManager;
        private readonly ILogger _logger;
        private readonly ICommandService _commandService;
        private readonly PowerController _powerController;

        public Program()
        {
            _logger = new Logger(LogLevel.Debug);
            _logger.AddTarget(new EchoTarget(Echo));
            _logger.AddTarget(new SurfaceTarget(Me.GetSurface(0)));
            _logger.Info("Logger initialized.");

            IConfigStorage storage = new ProgrammableBlockStorage(Me);
            _configManager = new ConfigurationManager(storage);

            _configManager.Register(new PowerManagementConfig());
            _configManager.Register(new LoggerConfig());

            try
            {
                _configManager.Load();
                _logger.Info("Configuration loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"CRITICAL: Failed to load config. {ex.Message}");
            }

            _powerController = new PowerController(_configManager, _logger);
            _logger.Info("Business logic modules initialized.");

            _commandService = new CommandService(_logger);
            _commandService.RegisterModule(_powerController);
            _logger.Info("Command service initialized and modules registered.");

            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            _logger.Info("--- Script startup complete ---");
            _logger.Info("Use 'power:status' or 'power:set_recharge <0-100>' to test.");
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if ((updateSource & (UpdateType.Terminal | UpdateType.Trigger | UpdateType.Mod)) != 0)
            {
                _commandService.Execute(argument);
            }

            if ((updateSource & UpdateType.Update100) != 0)
            {
                _powerController.ManagePower();
            }

            _logger.Flush();
        }
    }

    public class PowerController : ICommandModule
    {
        private readonly PowerManagementConfig _config;
        private readonly ILogger _logger;

        public PowerController(IConfigurationManager configManager, ILogger logger)
        {
            _config = configManager.GetOptions<PowerManagementConfig>();
            _logger = logger;
        }

        public void ManagePower()
        {
        }

        public IEnumerable<ICommand> GetCommands()
        {
            return new[]
            {
            new Command(
                "power:status",
                "Usage: power:status",
                ShowStatus),
            new Command(
                "power:set_recharge",
                "Usage: power:set_recharge <0-100>",
                SetRechargeThreshold)
        };
        }

        private void ShowStatus(string[] args)
        {
            if (args.Length != 0)
            {
                _logger.Error("Command 'power:status' does not accept any arguments.");
                return;
            }

            _logger.Info("--- Power Management Status ---");
            _logger.Info($"Battery Tag: '{_config.BatteryTag}'");
            _logger.Info($"Recharge when drops to: {_config.RechargeThreshold * 100:F0}%");
            _logger.Info($"Discharge until reaches: {_config.DischargeThreshold * 100:F0}%");
        }

        private void SetRechargeThreshold(string[] args)
        {
            if (args.Length != 1)
            {
                _logger.Error("Incorrect parameters. Usage: power:set_recharge <0-100>");
                return;
            }

            float percentage;
            if (!float.TryParse(args[0], out percentage) || percentage < 0 || percentage > 100)
            {
                _logger.Error($"Invalid value: '{args[0]}'. Must be a number between 0 and 100.");
                return;
            }

            _config.RechargeThreshold = percentage / 100.0f;

            _logger.Info($"Battery recharge threshold set to: {percentage}%");
        }
    }
}