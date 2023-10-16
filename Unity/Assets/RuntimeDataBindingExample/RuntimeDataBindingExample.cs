
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RuntimeDataBindingExample : MonoBehaviour
{
    protected void Start()
    {
        var dataSource = ScriptableObject.CreateInstance<ExampleObject>();

        var root = new VisualElement
        {
            name = "root",
            dataSource = dataSource
        };

        var vector3Field = new Vector3Field("Vec3 Field");

        vector3Field.SetBinding("label", new DataBinding
        {
            dataSourcePath = new PropertyPath(nameof(ExampleObject.vector3Label)),
            bindingMode = BindingMode.ToTarget
        });

        vector3Field.SetBinding("value", new DataBinding
        {
            dataSourcePath = new PropertyPath(nameof(ExampleObject.vector3Value))
        });

        root.Add(vector3Field);

        var floatField = new FloatField("Float Field") { value = 42.2f };

        floatField.SetBinding("value", new DataBinding
        {
            dataSourcePath = new PropertyPath(nameof(ExampleObject.sumOfVector3Properties))
        });

        root.Add(floatField);

        var label = new Label("Label")
        {
            dataSourcePath = new PropertyPath(nameof(ExampleObject.dangerLevel))
        };

        // Here, we do not need to set the dataSourcePath because we will only use two bindings and they will use the same path,
        // so we set the dataSourcePath on the Label directly instead.
        var binding = new DataBinding
        {
            bindingMode = BindingMode.ToTarget
        };

        // Add a custom float -> string converter
        binding.sourceToUiConverters.AddConverter((ref float v) => 
        {
            return v switch
            {
                >= 0 and < 1.0f/3.0f => "Danger",
                >= 1.0f/3.0f and < 2.0f/3.0f => "Neutral",
                _ => "Good"
            };
        });

        // Add a custom float -> StyleColor
        binding.sourceToUiConverters.AddConverter((ref float v) => new StyleColor(Color.Lerp(Color.red, Color.green, v)));

        // Since the binding is targeting the same data source property, we can re-use the same instance.
        label.SetBinding("text", binding);
        label.SetBinding("style.backgroundColor", binding);

        root.Add(label);
        
        Debug.Log ("label1: " + label.text);
        dataSource.dangerLevel = 0.5f;
        dataSource.vector3Label = "Hello World!";
        Debug.Log ("label2: " + label.text);
        Debug.Log("dataSource: " + dataSource.vector3Label);
    }
}
