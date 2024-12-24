#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;
using UnityEditor;
using System.IO;
using Sirenix.Utilities;

[assembly: RegisterValidationRule(typeof(MaterialNomenclatureValidator), Name = "Materiall NomenclatureValidator", Description = "Validates the correct naming of Materials (M_Name)")]

public class MaterialNomenclatureValidator : RootObjectValidator<Material>
{
    // Introduce serialized fields here to make your validator
    // configurable from the validator window under rules.
    public int SerializedConfig;

    protected override void Validate(ValidationResult result)
    {
         var obj = this.Object;
        var path = AssetDatabase.GetAssetPath(this.Object);
         if (!path.IsNullOrWhitespace() && !path.Contains("Packages")&& !obj.name.StartsWith("M_"))
         {
            result.AddError("Materials name should start with M_").WithFix(() => FixMaterialName(path,this.Object.name )) ;
         }
    }
    private void FixMaterialName(string materialPath,string name)
    {
        string directory = Path.GetDirectoryName(materialPath);
        string newPath = Path.Combine(directory, "M_"+name + ".mat");

        AssetDatabase.RenameAsset(materialPath, "M_" + name + ".mat");
        AssetDatabase.SaveAssets();
    }
}
#endif
