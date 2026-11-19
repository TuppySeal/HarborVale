using System;
using System.IO;
using System.Text;
using UnityEngine;
namespace HarborValeGreybox.Workshop {
public static class TurboAudioPreferenceStore {
 [Serializable]class Settings {public int version;public int releaseMode;}
 public static TurboReleaseMode Load(string path){if(!File.Exists(path))return TurboReleaseMode.BlowOffValve;var s=JsonUtility.FromJson<Settings>(File.ReadAllText(path));if(s==null||s.version!=1||s.releaseMode<0||s.releaseMode>1)throw new InvalidDataException("Turbo audio settings could not be read. Existing file preserved.");return (TurboReleaseMode)s.releaseMode;}
 public static TurboReleaseMode LoadMigrated(string path,string legacy){if(File.Exists(path)||legacy==null||!File.Exists(legacy))return Load(path);var choice=Load(legacy);Save(path,choice);return choice;}
 public static void Save(string path,TurboReleaseMode mode,Action beforeReplace=null){if(mode!=TurboReleaseMode.BlowOffValve&&mode!=TurboReleaseMode.Flutter)throw new ArgumentException("Invalid turbo sound choice");Load(path);Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));string temp=path+".tmp";byte[] bytes=Encoding.UTF8.GetBytes(JsonUtility.ToJson(new Settings{version=1,releaseMode=(int)mode},true));using(var f=new FileStream(temp,FileMode.Create,FileAccess.Write,FileShare.None)){f.Write(bytes,0,bytes.Length);f.Flush(true);}beforeReplace?.Invoke();if(File.Exists(path))File.Replace(temp,path,null);else File.Move(temp,path);}
}
}
