
// Default include for Ambient light definition
float4 AmbientColor : Ambient
<
	string SasBindAddress = "Sas.AmbientLights[0].Color";
	string SasUiControl = "ColorPicker";
	string UIWidget = "Color";
	string UIName =  "Ambient Color/Light";
> = {0.3f, 0.3f, 0.3f, 1.0f};