using System;
namespace ClaudeGaragePreview {
[Serializable]public class UpgradePart {
 public string id,displayName,description;
 public UpgradePart(string id,string name,string description){this.id=id;displayName=name;this.description=description;}
}
}
