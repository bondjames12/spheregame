// Material.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace QuickStart.Graphics
{
    /// <summary>
    /// The QuickStart material class.  Materials be loaded through the content pipeline or directly at run-time from the .qsm files.
    /// </summary>
    public class Material
    {
        // TODO:  Find a better way to store these parameter -> value mappings

        /// <summary>
        /// Shader Parameter to Texture Binding.
        /// </summary>
        private struct TextureBinding
        {
            public EffectParameter parameter;
            public Texture texture;
        }

        /// <summary>
        /// Shader Parameter to Contant Float Binding.
        /// </summary>
        private struct ConstantFloatBinding
        {
            public EffectParameter parameter;
            public float constant;
        }

        /// <summary>
        /// Shader Parameter to Constant Float4 Binding.
        /// </summary>
        private struct ConstantFloat4Binding
        {
            public EffectParameter parameter;
            public Vector4 constant;
        }

        /// <summary>
        /// Shader Parameter to Constant Matrix Binding.
        /// </summary>
        private struct ConstantMatrixBinding
        {
            public EffectParameter parameter;
            public Matrix constant;
        }

        /// <summary>
        /// Shader Parameter to Variable Float Binding.
        /// </summary>
        private struct VariableFloatBinding
        {
            public EffectParameter parameter;
            public GraphicsSystem.VariableFloatID varID;
        }

        /// <summary>
        /// Shader Parameter to Variable Float4 Binding.
        /// </summary>
        private struct VariableFloat4Binding
        {
            public EffectParameter parameter;
            public GraphicsSystem.VariableFloat4ID varID;
        }

        /// <summary>
        /// Shader Parameter to Variable Matrix Binding.
        /// </summary>
        private struct VariableMatrixBinding
        {
            public EffectParameter parameter;
            public GraphicsSystem.VariableMatrixID matrixID;
        }

        private Effect effect;
        private List<TextureBinding> textureBindings;
        private List<ConstantFloatBinding> constantFloatBindings;
        private List<ConstantFloat4Binding> constantFloat4Bindings;
        private List<ConstantMatrixBinding> constantMatrixBindings;
        private List<VariableFloatBinding> variableFloatBindings;
        private List<VariableFloat4Binding> variableFloat4Bindings;
        private List<VariableMatrixBinding> variableMatrixBindings;

        private EffectPass currPass;

        /// <summary>
        /// Initializes a new instance of a material.  Only callable from internal library code.
        /// </summary>
        internal Material()
        {
            this.textureBindings = new List<TextureBinding>();
            this.constantFloatBindings = new List<ConstantFloatBinding>();
            this.constantFloat4Bindings = new List<ConstantFloat4Binding>();
            this.constantMatrixBindings = new List<ConstantMatrixBinding>();
            this.variableFloatBindings = new List<VariableFloatBinding>();
            this.variableFloat4Bindings = new List<VariableFloat4Binding>();
            this.variableMatrixBindings = new List<VariableMatrixBinding>();
        }

        /// <summary>
        /// Initializes a new instance of a material from a non-compiled file on disk.
        /// </summary>
        /// <param name="filename">The material file (*.qsm) to load.</param>
        /// <param name="content">The ContentManager instance to use for asset loading.</param>
        public Material(string filename, ContentManager content)
        {
            this.textureBindings = new List<TextureBinding>();
            this.constantFloatBindings = new List<ConstantFloatBinding>();
            this.constantFloat4Bindings = new List<ConstantFloat4Binding>();
            this.constantMatrixBindings = new List<ConstantMatrixBinding>();
            this.variableFloatBindings = new List<VariableFloatBinding>();
            this.variableFloat4Bindings = new List<VariableFloat4Binding>();
            this.variableMatrixBindings = new List<VariableMatrixBinding>();

            if(!LoadFromFile(filename, content))
            {
                throw new Exception("Failed to load material from disk.");
            }
        }

        /// <summary>
        /// Binds the material to the graphics device for rendering.
        /// </summary>
        /// <param name="graphics">The game's GraphicsSystem instance.</param>
        /// <returns>The number of passes required for this material.</returns>
        public int BindMaterial(GraphicsSystem graphics)
        {
            // Iterate over parameters and bind.

            // Bind textures
            for(int i = 0; i < this.textureBindings.Count; ++i)
            {
                this.textureBindings[i].parameter.SetValue(this.textureBindings[i].texture);
            }

            // Bind constants
            for(int i = 0; i < this.constantFloatBindings.Count; ++i)
            {
                this.constantFloatBindings[i].parameter.SetValue(this.constantFloatBindings[i].constant);
            }

            // Bind constants
            for(int i = 0; i < this.constantFloat4Bindings.Count; ++i)
            {
                this.constantFloat4Bindings[i].parameter.SetValue(this.constantFloat4Bindings[i].constant);
            }

            // Bind constants
            for(int i = 0; i < this.constantMatrixBindings.Count; ++i)
            {
                this.constantMatrixBindings[i].parameter.SetValue(this.constantMatrixBindings[i].constant);
            }

            for(int i = 0; i < this.variableFloatBindings.Count; ++i)
            {
                float val;
                graphics.GetVariableFloat(this.variableFloatBindings[i].varID, out val);
                this.variableFloatBindings[i].parameter.SetValue(val);
            }

            for(int i = 0; i < this.variableFloat4Bindings.Count; ++i)
            {
                Vector4 val;
                graphics.GetVariableFloat4(this.variableFloat4Bindings[i].varID, out val);
                this.variableFloat4Bindings[i].parameter.SetValue(val);
            }

            // Bind render matrices
            for(int i = 0; i < this.variableMatrixBindings.Count; ++i)
            {
                Matrix mat;
                graphics.GetVariableMatrix(this.variableMatrixBindings[i].matrixID, out mat);
                this.variableMatrixBindings[i].parameter.SetValue(mat);
            }

            this.effect.Begin();
            return this.effect.CurrentTechnique.Passes.Count;
        }

        /// <summary>
        /// Unbinds the material from the graphics device.
        /// </summary>
        public void UnBindMaterial()
        {
            this.effect.End();
        }

        /// <summary>
        /// Sets up shaders for specified rendering pass.
        /// </summary>
        /// <param name="i">The material rendering pass.</param>
        public void BeginPass(int i)
        {
            this.currPass = effect.CurrentTechnique.Passes[i];
            this.currPass.Begin();
        }

        /// <summary>
        /// Finalizes rendering for the current pass.
        /// </summary>
        public void EndPass()
        {
            this.currPass.End();
        }

        /// <summary>
        /// Loads a non-compiled material directly from disk.
        /// </summary>
        /// <param name="name">The material file to load.</param>
        /// <param name="content">The ContentManager instance to use for asset loading.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool LoadFromFile(string name, ContentManager content)
        {
            XmlTextReader reader = new XmlTextReader(name);
            XmlDocument document = new XmlDocument();
            document.Load(reader);
            reader.Close();


            if (ParseRootXMLNode(document.DocumentElement, content) == false)
            {
                return false;
            }


            if (effect == null)
            {
                throw new Exception("Error in material file: Every material must have an effect.");
            }


            return true;
        }

        /// <summary>
        /// Parses the root XML node of the material file, then recurses for children.
        /// </summary>
        /// <param name="node">The root XML node.</param>
        /// <param name="content">The ContentManager instance to use for asset loading.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool ParseRootXMLNode(XmlNode node, ContentManager content)
        {
            switch(node.Name)
            {
                case "material":
                {
                    XmlNode effectNode = node.Attributes.GetNamedItem("effect");
                    if (effectNode == null)
                    {
                        throw new Exception("Error in material file: Root <material> node must have effect attribute.");
                    }

                    string name = effectNode.InnerText;

                    this.effect = content.Load<Effect>(string.Format("Effects/{0}", name));
                    if (this.effect == null)
                    {
                        throw new Exception("Error in material file: failed to load effect.");
                    }

                    foreach(XmlNode child in node.ChildNodes)
                    {
                        if (!ParseXMLBodyNode(child, content))
                        {
                            return false;
                        }
                    }
                    break;
                }
                default:
                {
                    throw new Exception("Error in material file: Root node is not <material>.");
                }
            }
            return true;
        }

        /// <summary>
        /// Parses an XML node in the material file.
        /// </summary>
        /// <param name="node">The XML node to parse.</param>
        /// <param name="content">The ContentManager instance to use for asset loading.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool ParseXMLBodyNode(XmlNode node, ContentManager content)
        {
            switch(node.Name)
            {
                case "sampler":
                {
                    // Parse sampler

                    if (node.ChildNodes.Count > 1)
                    {
                        throw new Exception("Error in material file: <sampler> node cannot have children nodes.");
                    }

                    XmlNode semanticNode = node.Attributes.GetNamedItem("semantic");
                    if (semanticNode == null)
                    {
                        throw new Exception("Error in material file: <sampler> node must have semantic usage.");
                    }

                    Texture tex = content.Load<Texture>(string.Format("Textures/{0}", node.InnerText));

                    if (tex == null)
                    {
                        throw new Exception("Error in material file: failed to load texture.");
                    }

                    EffectParameter param = this.effect.Parameters.GetParameterBySemantic(semanticNode.InnerText);

                    if (param == null)
                    {
                        throw new Exception("Error in material file: Sampler semantic not found in effect.");
                    }

                    TextureBinding binding = new TextureBinding();
                    binding.parameter = param;
                    binding.texture = tex;

                    this.textureBindings.Add(binding);

                    break;
                }
                case "variable_float":
                {
                    // Parse matrix variable

                    if (node.ChildNodes.Count > 1)
                    {
                        throw new Exception("Error in material file: <variable_float> node cannot have children nodes.");
                    }

                    XmlNode semanticNode = node.Attributes.GetNamedItem("semantic");
                    if (semanticNode == null)
                    {
                        throw new Exception("Error in material file: <variable_float> node must have semantic usage.");
                    }

                    XmlNode matIDNode = node.Attributes.GetNamedItem("varID");
                    if (matIDNode == null)
                    {
                        throw new Exception("Error in material file: <variable_float> node must have rendervar attribute.");
                    }

                    EffectParameter param = this.effect.Parameters.GetParameterBySemantic(semanticNode.InnerText);

                    if (param == null)
                    {
                        throw new Exception("Error in material file: semantic not found in effect.");
                    }

                    GraphicsSystem.VariableFloatID matID;

                    try
                    {
                        matID = (GraphicsSystem.VariableFloatID)Enum.Parse(typeof(GraphicsSystem.VariableFloatID), matIDNode.InnerText, true);
                    }
                    catch(Exception)
                    {
                        throw new Exception("Error in material file: <variable_float> node contained unknown rendervar.");
                    }

                    VariableFloatBinding binding = new VariableFloatBinding();
                    binding.varID = matID;
                    binding.parameter = param;

                    this.variableFloatBindings.Add(binding);

                    break;
                }
                case "variable_float4":
                {
                    // Parse matrix variable

                    if (node.ChildNodes.Count > 1)
                    {
                        throw new Exception("Error in material file: <variable_float4> node cannot have children nodes.");
                    }

                    XmlNode semanticNode = node.Attributes.GetNamedItem("semantic");
                    if (semanticNode == null)
                    {
                        throw new Exception("Error in material file: <variable_float4> node must have semantic usage.");
                    }

                    XmlNode matIDNode = node.Attributes.GetNamedItem("varID");
                    if (matIDNode == null)
                    {
                        throw new Exception("Error in material file: <variable_float4> node must have rendervar attribute.");
                    }

                    EffectParameter param = this.effect.Parameters.GetParameterBySemantic(semanticNode.InnerText);

                    if (param == null)
                    {
                        throw new Exception("Error in material file: semantic not found in effect.");
                    }

                    GraphicsSystem.VariableFloat4ID matID;

                    try
                    {
                        matID = (GraphicsSystem.VariableFloat4ID)Enum.Parse(typeof(GraphicsSystem.VariableFloat4ID), matIDNode.InnerText, true);
                    }
                    catch(Exception)
                    {
                        throw new Exception("Error in material file: <variable_float4> node contained unknown rendervar.");
                    }

                    VariableFloat4Binding binding = new VariableFloat4Binding();
                    binding.varID = matID;
                    binding.parameter = param;

                    this.variableFloat4Bindings.Add(binding);

                    break;
                }
                case "variable_matrix":
                {
                    // Parse matrix variable

                    if (node.ChildNodes.Count > 1)
                    {
                        throw new Exception("Error in material file: <variable_matrix> node cannot have children nodes.");
                    }

                    XmlNode semanticNode = node.Attributes.GetNamedItem("semantic");
                    if (semanticNode == null)
                    {
                        throw new Exception("Error in material file: <variable_matrix> node must have semantic usage.");
                    }

                    XmlNode matIDNode = node.Attributes.GetNamedItem("varID");
                    if (matIDNode == null)
                    {
                        throw new Exception("Error in material file: <variable_matrix> node must have varID attribute.");
                    }

                    EffectParameter param = this.effect.Parameters.GetParameterBySemantic(semanticNode.InnerText);

                    if (param == null)
                    {
                        throw new Exception("Error in material file: semantic not found in effect.");
                    }

                    GraphicsSystem.VariableMatrixID matID;

                    try
                    {
                        matID = (GraphicsSystem.VariableMatrixID)Enum.Parse(typeof(GraphicsSystem.VariableMatrixID), matIDNode.InnerText, true);
                    }
                    catch(Exception)
                    {
                        throw new Exception("Error in material file: <variable_matrix> node contained unknown rendervar.");
                    }

                    VariableMatrixBinding binding = new VariableMatrixBinding();
                    binding.matrixID = matID;
                    binding.parameter = param;

                    this.variableMatrixBindings.Add(binding);

                    break;
                }
                case "constant_float":
                {
                    // Parse constant variable

                    if (node.ChildNodes.Count > 1)
                    {
                        throw new Exception("Error in material file: <constant_float1> node cannot have children nodes.");
                    }

                    XmlNode semanticNode = node.Attributes.GetNamedItem("semantic");
                    if (semanticNode == null)
                    {
                        throw new Exception("Error in material file: <constant_float1> node must have semantic usage.");
                    }

#if XBOX360
                    float value = 1.0f;
#else
                    float value;
                    if(float.TryParse(node.InnerText, NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value) == false)
                    {
                        throw new Exception("Error in material file: <constant_float1> node only supports floating-point constants.");
                    }
#endif

                    EffectParameter param = this.effect.Parameters.GetParameterBySemantic(semanticNode.InnerText);
                    if (param == null)
                    {
                        throw new Exception("Error in material file: effect does not contain semantic for <constant_float1> node.");
                    }


                    ConstantFloatBinding binding = new ConstantFloatBinding();
                    binding.constant = value;
                    binding.parameter = param;

                    this.constantFloatBindings.Add(binding);

                    break;
                }

                case "constant_float4":
                {
                    // Parse constant variable

                    if (node.ChildNodes.Count > 1)
                    {
                        throw new Exception("Error in material file: <constant_float4> node cannot have children nodes.");
                    }

                    XmlNode semanticNode = node.Attributes.GetNamedItem("semantic");
                    if (semanticNode == null)
                    {
                        throw new Exception("Error in material file: <constant_float4> node must have semantic usage.");
                    }

                    Vector4 value;

                    string[] vars = node.InnerText.Split(',');
                    if (vars.Length != 4)
                    {
                        throw new Exception("Error in material file: <constant_float4> node must specify 4 floats in the form: f1, f2, f3, f4");
                    }

#if XBOX360
                    value = Vector4.One;
#else
                    if((float.TryParse(vars[0], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.X) == false) 
                      || (float.TryParse(vars[1], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.Y) == false) 
                      || (float.TryParse(vars[2], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.Z) == false)
                      || (float.TryParse(vars[3], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.W) == false))
                    {
                        throw new Exception("Error in material file: <constant_float4> node only supports floating-point constants.");
                    }
#endif

                    EffectParameter param = this.effect.Parameters.GetParameterBySemantic(semanticNode.InnerText);
                    if(param == null)
                    {
                        throw new Exception("Error in material file: effect does not contain semantic for <constant_float4> node.");
                    }

                    ConstantFloat4Binding binding = new ConstantFloat4Binding();
                    binding.constant = value;
                    binding.parameter = param;

                    this.constantFloat4Bindings.Add(binding);

                    break;
                }

                case "constant_matrix":
                {
                    // Parse constant variable

                    if(node.ChildNodes.Count > 1)
                    {
                        throw new Exception("Error in material file: <constant_matrix> node cannot have children nodes.");
                    }

                    XmlNode semanticNode = node.Attributes.GetNamedItem("semantic");
                    if(semanticNode == null)
                    {
                        throw new Exception("Error in material file: <constant_matrix> node must have semantic usage.");
                    }

                    Matrix value;

                    string[] vars = node.InnerText.Split(',');
                    if(vars.Length != 16)
                    {
                        throw new Exception("Error in material file: <constant_matrix> node must specify 4 floats in the form: f1, f2, f3, f4, ..., f16");
                    }

#if XBOX360
                    value = Matrix.Identity;
#else
                    if(!float.TryParse(vars[0], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M11) || !float.TryParse(vars[1], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M12) || !float.TryParse(vars[2], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M13) || !float.TryParse(vars[3], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M14) ||
                       !float.TryParse(vars[4], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M21) || !float.TryParse(vars[5], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M22) || !float.TryParse(vars[6], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M23) || !float.TryParse(vars[7], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M24) ||
                       !float.TryParse(vars[8], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M31) || !float.TryParse(vars[9], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M32) || !float.TryParse(vars[10], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M33) || !float.TryParse(vars[11], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M34) ||
                       !float.TryParse(vars[12], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M41) || !float.TryParse(vars[13], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M42) || !float.TryParse(vars[14], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M43) || !float.TryParse(vars[15], NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out value.M44))
                    {
                        throw new Exception("Error in material file: <constant_matrix> node only supports floating-point constants.");
                    }
#endif

                    EffectParameter param = this.effect.Parameters.GetParameterBySemantic(semanticNode.InnerText);
                    if(param == null)
                    {
                        throw new Exception("Error in material file: effect does not contain semantic for <constant_matrix> node.");
                    }

                    ConstantMatrixBinding binding = new ConstantMatrixBinding();
                    binding.constant = value;
                    binding.parameter = param;

                    this.constantMatrixBindings.Add(binding);

                    break;
                }


                default:
                {
                    // Ignore unknown node
                    // TODO: Warning message
                    break;
                }
            }
            return true;
        }


        /// <summary>
        /// Loads a compiled material file from a content pipeline data file.
        /// </summary>
        /// <param name="reader">The ContentReader instance for the data file.</param>
        public void LoadFromContent(ContentReader reader)
        {
            // Read referenced effect
            this.effect = reader.ReadExternalReference<Effect>();

            int numConstants = reader.ReadInt32();
            for(int i = 0; i < numConstants; ++i)
            {
                string semantic = reader.ReadString();
                int numValues = reader.ReadInt32();

                if(numValues == 1)
                {
                    float val = reader.ReadSingle();

                    ConstantFloatBinding binding = new ConstantFloatBinding();
                    binding.constant = val;
                    binding.parameter = this.effect.Parameters.GetParameterBySemantic(semantic);
                    if (binding.parameter == null)
                    {
                        throw new Exception("Error in material: Undefined semantic.");
                    }

                    this.constantFloatBindings.Add(binding);
                }
                else if(numValues == 4)
                {
                    Vector4 val = new Vector4();
                    val.X = reader.ReadSingle();
                    val.Y = reader.ReadSingle();
                    val.Z = reader.ReadSingle();
                    val.W = reader.ReadSingle();

                    ConstantFloat4Binding binding = new ConstantFloat4Binding();
                    binding.constant = val;
                    binding.parameter = this.effect.Parameters.GetParameterBySemantic(semantic);
                    if (binding.parameter == null)
                    {
                        throw new Exception("Error in material: Undefined semantic.");
                    }

                    this.constantFloat4Bindings.Add(binding);
                }
                else if(numValues == 16)
                {
                    Matrix val = new Matrix();
                    val.M11 = reader.ReadSingle();
                    val.M12 = reader.ReadSingle();
                    val.M13 = reader.ReadSingle();
                    val.M14 = reader.ReadSingle();

                    val.M21 = reader.ReadSingle();
                    val.M22 = reader.ReadSingle();
                    val.M23 = reader.ReadSingle();
                    val.M24 = reader.ReadSingle();

                    val.M31 = reader.ReadSingle();
                    val.M32 = reader.ReadSingle();
                    val.M33 = reader.ReadSingle();
                    val.M34 = reader.ReadSingle();

                    val.M41 = reader.ReadSingle();
                    val.M42 = reader.ReadSingle();
                    val.M43 = reader.ReadSingle();
                    val.M44 = reader.ReadSingle();

                    ConstantMatrixBinding binding = new ConstantMatrixBinding();
                    binding.constant = val;
                    binding.parameter = this.effect.Parameters.GetParameterBySemantic(semantic);
                    if (binding.parameter == null)
                    {
                        throw new Exception("Error in material: Undefined semantic.");
                    }

                    this.constantMatrixBindings.Add(binding);
                }
                else
                    throw new Exception("Error loading material: Invalid constant size.");
            }

            int numVariables = reader.ReadInt32();

            for(int i = 0; i < numVariables; ++i)
            {
                string semantic = reader.ReadString();
                string variable = reader.ReadString();

                int idx = variable.IndexOf(':');
                string type = variable.Substring(0, idx);
                string varName = variable.Substring(idx + 1);

                switch(type)
                {
                    case "Sampler":
                    {
                        TextureBinding binding = new TextureBinding();
                        binding.parameter = this.effect.Parameters.GetParameterBySemantic(semantic);
                        if (binding.parameter == null)
                        {
                            throw new Exception("Error in material: Undefined semantic.");
                        }

                        binding.texture = reader.ContentManager.Load<Texture>(string.Format("Textures/{0}", varName));
                        this.textureBindings.Add(binding);
                        break;
                    }


                    case "Matrix":
                    {
                        VariableMatrixBinding binding = new VariableMatrixBinding();

                        binding.parameter = this.effect.Parameters.GetParameterBySemantic(semantic);
                        if (binding.parameter == null)
                        {
                            throw new Exception("Error in material: Undefined semantic.");
                        }


                        try
                        {
                            binding.matrixID = (GraphicsSystem.VariableMatrixID)Enum.Parse(typeof(GraphicsSystem.VariableMatrixID), varName, true);
                        }
                        catch(Exception)
                        {
                            throw new Exception("Error in material file: <variable_matrix> node contained unknown rendervar.");
                        }

                        this.variableMatrixBindings.Add(binding);

                        break;
                    }

                    case "Float4":
                    {
                        VariableFloat4Binding binding = new VariableFloat4Binding();

                        binding.parameter = this.effect.Parameters.GetParameterBySemantic(semantic);
                        if (binding.parameter == null)
                        {
                            throw new Exception("Error in material: Undefined semantic.");
                        }


                        try
                        {
                            binding.varID = (GraphicsSystem.VariableFloat4ID)Enum.Parse(typeof(GraphicsSystem.VariableFloat4ID), varName, true);
                        }
                        catch(Exception)
                        {
                            throw new Exception("Error in material file: <variable_float4> node contained unknown rendervar.");
                        }

                        this.variableFloat4Bindings.Add(binding);
                        break;
                    }

                    case "Float":
                    {
                        VariableFloatBinding binding = new VariableFloatBinding();

                        binding.parameter = this.effect.Parameters.GetParameterBySemantic(semantic);
                        if (binding.parameter == null)
                        {
                            throw new Exception("Error in material: Undefined semantic.");
                        }


                        try
                        {
                            binding.varID = (GraphicsSystem.VariableFloatID)Enum.Parse(typeof(GraphicsSystem.VariableFloatID), varName, true);
                        }
                        catch(Exception)
                        {
                            throw new Exception("Error in material file: <variable_float> node contained unknown rendervar.");
                        }

                        this.variableFloatBindings.Add(binding);
                        break;
                    }
                }
            }
        }
    }
}
