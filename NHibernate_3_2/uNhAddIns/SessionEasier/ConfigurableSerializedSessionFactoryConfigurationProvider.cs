using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg;

namespace uNhAddIns.SessionEasier
{
   /// <summary>
   /// Serialize the first time the configuration to a file. 
   /// Use the serialized version until the assembly or configuration change.
   /// This class was extracted from the Effectus sample of Ayende:
   /// http://msdn.microsoft.com/en-us/magazine/ee819139.aspx
   /// </summary>
   public class ConfigurableSerializedSessionFactoryConfigurationProvider : AbstractConfigurationProvider
   {
      /// <summary>
      /// files to check if the configuration serialization must be re-created
      /// </summary>
      private readonly string[] dependencyFiles;
      private readonly string serializedConfiguration = "configuration.serialized";

      public ConfigurableSerializedSessionFactoryConfigurationProvider()
      {
         this.dependencyFiles = new string[] { };
      }

      public ConfigurableSerializedSessionFactoryConfigurationProvider(
         string serializedConfiguration,
         string[] dependencyFiles)
      {
         this.serializedConfiguration = serializedConfiguration;
         this.dependencyFiles = dependencyFiles;
      }

<<<<<<< HEAD
      public string SerializedConfiguration
      {
         get { return serializedConfiguration; }
      }
=======
>>>>>>> dcb9380f740256607883b74d651ec24ee0c111f0

      private bool IsConfigurationFileValid
      {
         get
         {
            var configInfo = new FileInfo(serializedConfiguration);
            foreach (var configFile in dependencyFiles)
            {
               var configFileInfo = new FileInfo(configFile);
               if (configInfo.LastWriteTime < configFileInfo.LastWriteTime)
               {
                  return false;
               }
            }

            return true;
         }
      }

      public override IEnumerable<Configuration> Configure()
      {
         var configuration = new Configuration();

         bool configured;
         DoBeforeConfigure(configuration, out configured);

         if (!configured)
         {
            configuration = LoadConfigurationFromFile();

            if (configuration == null)
            {
               configuration = CreateConfiguration();

               DoAfterConfigure(configuration);
               SaveConfigurationToFile(configuration);
            }
         }
         return new List<Configuration> { configuration };
      }

      private void SaveConfigurationToFile(Configuration configuration)
      {
         using (FileStream file = File.Open(serializedConfiguration, FileMode.Create))
         {
            var bf = new BinaryFormatter();
            bf.Serialize(file, configuration);
         }
      }

      private Configuration LoadConfigurationFromFile()
      {
         if (IsConfigurationFileValid == false)
            return null;
         try
         {
            using (var file = File.Open(serializedConfiguration, FileMode.Open))
            {
               var bf = new BinaryFormatter();
               return bf.Deserialize(file) as Configuration;
            }
         }
         catch (Exception)
         {
            return null;
         }
      }
   }
}
