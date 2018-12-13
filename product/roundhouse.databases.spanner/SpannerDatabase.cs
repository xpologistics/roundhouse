using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Google.Cloud.Spanner.Data;
using roundhouse.connections;
using roundhouse.consoles;


namespace roundhouse.databases.spanner
{
    using infrastructure.app;
    using infrastructure.extensions;
    using infrastructure.logging;
    using roundhouse.infrastructure.app;

    public class SpannerDatabase : AdoNetDatabase
    {
        public override void initialize_connections(ConfigurationPropertyHolder configuration_property_holder)
        {
            if (string.IsNullOrEmpty(connection_string))
            {
                throw new Exception("Provide Spanner ConnectionString");
            }

            set_provider();
        }

        public override void set_provider()
        {
            provider = "Google.Cloud.Spanner.Data";
        }

        public override string create_database_script()
        {
            return string.Empty;
        }

        public override string delete_database_script()
        {
            return string.Empty;
        }

        public override string set_recovery_mode_script(bool simple)
        {
            return string.Empty;
        }

        public override string restore_database_script(string restore_from_path, string custom_restore_options)
        {
            throw new NotImplementedException();
        }


        public override void run_database_specific_tasks()
        {
            Log.bound_to(this).log_a_debug_event_containing("Spanner has no database specific tasks. Continuing ...");
        }

        public override void open_connection(bool with_transaction)
        {
            server_connection = new AdoNetConnection(new SpannerConnection(connection_string));
            //server_connection.open();
        }

        public override void run_sql(string sql_to_run, ConnectionType connection_type)
        {
            if (string.IsNullOrEmpty(sql_to_run))
                return;

            var connection = server_connection.underlying_type().downcast_to<SpannerConnection>();

            var ddl_command = connection.CreateDdlCommand(sql_to_run);

            ddl_command.ExecuteNonQuery();
        }
    }
}
