using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

#if XBOX

#else
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
#endif

namespace XSIXNARuntime
{
    public class XSIAnimationKeyframe
    {
        public TimeSpan Time;
        public Matrix Transform;
        public XSIAnimationKeyframe(TimeSpan in_Time, Matrix in_Transform)
        {
            Time = in_Time;
            Transform = in_Transform;
        }
    };

    public class XSIAnimationChannel
    {
        public List<XSIAnimationKeyframe> KeyFrames;
        public ModelBone Target = null;
        public int CurrentFrame = 0;

        // this playback doesn't do interpolation
        public void PlayBack(TimeSpan Time, float blend)
        {
            int count = KeyFrames.Count;

            // check for no frame
            if(count == 0)
                return;

            Matrix l_Matrix = Target.Transform;

            // check for out of bounds
            if (Time <= KeyFrames[0].Time)
            {
                Target.Transform = KeyFrames[0].Transform;
            }
            else if (Time >= KeyFrames[count-1].Time)
            {
                Target.Transform = KeyFrames[count - 1].Transform;
            }
            // check if CurrentFrame is close
            else if ((CurrentFrame < (count - 1)) && (Time >= KeyFrames[CurrentFrame].Time) && (Time <= KeyFrames[CurrentFrame + 1].Time))
            {
                Target.Transform = KeyFrames[CurrentFrame].Transform;
            }
            else if (Time < KeyFrames[CurrentFrame].Time) // check if CurrentFrame is too high  
            {
                CurrentFrame = 0;
                while ((Time > KeyFrames[CurrentFrame].Time) && (CurrentFrame < (count - 1)))
                {
                    CurrentFrame++;
                }

                Target.Transform = KeyFrames[CurrentFrame].Transform;
            }
            else if (Time > KeyFrames[CurrentFrame].Time) // check if CurrentFrame is too low  
            {
                while ((Time > KeyFrames[CurrentFrame+1].Time) && (CurrentFrame < (count - 1)))
                {
                    CurrentFrame++;
                }

                Target.Transform = KeyFrames[CurrentFrame].Transform;
            }

            if (blend < 1.0f)
            {
                Target.Transform = Matrix.Lerp(l_Matrix, Target.Transform, blend);
            }
        }
    };

    public class XSIAnimationContent
    {
        public Dictionary<String, XSIAnimationChannel> Channels;
        public String Name;
        public TimeSpan Duration;
        public bool Loop = true;
        public TimeSpan CurrentTime;

        public void BindModelBones(Model in_Model)
        {
            foreach (ModelBone Bone in in_Model.Bones)
            {
                if (Channels.ContainsKey(Bone.Name))
                {
                    Channels[Bone.Name].Target = Bone;
                }
            }
        }

        public void PlayBack(TimeSpan Time, float blend)
        {
            CurrentTime += Time;
            if (Loop && (CurrentTime > Duration))
                CurrentTime -= Duration;

            foreach (KeyValuePair<String, XSIAnimationChannel> Channel in Channels)
            {
                Channel.Value.PlayBack(CurrentTime, blend);
            }
        }

        public void Reset()
        {
            CurrentTime = TimeSpan.Parse("0");
        }

    };

    public class XSIAnimationData
    {
#if XBOX
#else
        public AnimationContentDictionary  AnimationContentDictionary;
#endif
        public Dictionary<String, XSIAnimationContent> RuntimeAnimationContentDictionary;
        public List<String> BoneNames;
        public List<Matrix> BoneInvBindPoses;
        public Matrix[] BoneTransforms;
        public List<ModelBone> Bones;
        public String Header1;
        public String Header2;
        public String Header3;

        public XSIAnimationData()
        {
            Header1 = "XSIBINDPOSE";
            Header2 = "XSIANIM";
            Header3 = "XSIEND";
        }

        public void ResolveBones(Model in_Model)
        {
            Bones = new List<ModelBone>();
            BoneTransforms = new Matrix[BoneInvBindPoses.Count];
            foreach (String BoneName in BoneNames)
            {
                Bones.Add(in_Model.Bones[BoneName]);
            }
        }

        public void ComputeBoneTransforms(Matrix[] in_Transforms)
        {
            if (Bones != null)
            {
                int index = 0;
                int loop, count = Bones.Count;
                for (loop = 0; loop < count; loop++)
                {
                    BoneTransforms[index] = BoneInvBindPoses[index] * in_Transforms[Bones[index].Index];
                    index++;
                }
            }
        }
    };

    public class XSIAnimationDataReader : ContentTypeReader<XSIAnimationData>
    {
        protected override XSIAnimationData Read(ContentReader input,
                                             XSIAnimationData existingInstance)
        {
            XSIAnimationData l_Animations = new XSIAnimationData();
            l_Animations.RuntimeAnimationContentDictionary = new Dictionary<String, XSIAnimationContent>();

            // read the header
            l_Animations.Header1 = input.ReadObject<String>();

            // read the inverse bind poses
            l_Animations.BoneInvBindPoses = new List<Matrix>();
            l_Animations.BoneNames = new List<String>();
            int count = input.ReadObject<int>();
            int loop;

            // read each AnimationContent
            for (loop = 0; loop < count; loop++)
            {
                Matrix l_InverseBindPose = input.ReadObject<Matrix>();
                l_Animations.BoneInvBindPoses.Add(l_InverseBindPose);
            }

            for (loop = 0; loop < count; loop++)
            {
                String l_BoneName = input.ReadObject<String>();
                l_Animations.BoneNames.Add(l_BoneName);
            }

            // read the header
            l_Animations.Header2 = input.ReadObject<String>();
            count = input.ReadObject<int>();

            // read each AnimationContent
            for (loop = 0; loop < count; loop++)
            {
                XSIAnimationContent l_AnimationContent = new XSIAnimationContent();
                l_AnimationContent.Channels = new Dictionary<string,XSIAnimationChannel>();

                String AnimationContentKey = input.ReadObject<String>();
                l_AnimationContent.Name = input.ReadObject<String>();
                l_AnimationContent.Duration = input.ReadObject<TimeSpan>();

                l_Animations.RuntimeAnimationContentDictionary.Add(AnimationContentKey, l_AnimationContent);

                // read each AnimationChannel
                int channelcount = input.ReadObject<int>();
                int channelloop;
                for (channelloop = 0; channelloop < channelcount; channelloop++)
                {
                    String AnimationChannelKey = input.ReadObject<String>();
                    int AnimationKeyFrameCount = input.ReadObject<int>();

                    XSIAnimationChannel l_AnimationChannel = new XSIAnimationChannel();
                    l_AnimationChannel.KeyFrames = new List<XSIAnimationKeyframe>();

                    int keyframeloop;

                    // read each keyframe
                    for (keyframeloop = 0; keyframeloop < AnimationKeyFrameCount; keyframeloop++)
                    {
                        XSIAnimationKeyframe l_KeyFrame = new XSIAnimationKeyframe(input.ReadObject<TimeSpan>(), input.ReadObject<Matrix>());
                        l_AnimationChannel.KeyFrames.Add(l_KeyFrame);
                    }

                    l_AnimationContent.Channels.Add(AnimationChannelKey, l_AnimationChannel);
                }
            }

            // read the footer
            l_Animations.Header3 = input.ReadObject<String>();
            return l_Animations;
        }
    }

#if XBOX
#else
    [ContentTypeWriter]

    public class XSIAnimationDataWriter : ContentTypeWriter<XSIAnimationData>
    {
        protected override void Write(ContentWriter output, XSIAnimationData value)
        {
	        // write the header
	        output.WriteObject(value.Header1);

            // write the inverse bind poses
            output.WriteObject(value.BoneInvBindPoses.Count);

            foreach (Matrix l_Matrix in value.BoneInvBindPoses)
            {
                output.WriteObject(l_Matrix);
            }

            foreach (String l_BoneName in value.BoneNames)
            {
                output.WriteObject(l_BoneName);
            }

            // write the header
            output.WriteObject(value.Header2);

	        // write the AnimationContentDictionary properties
	        output.WriteObject(value.AnimationContentDictionary.Count);

	        // write each AnimationContent
	        foreach ( System.Collections.Generic.KeyValuePair<String , AnimationContent> l_AnimationContent in value.AnimationContentDictionary)
	        {
		        output.WriteObject(l_AnimationContent.Key);
		        output.WriteObject(l_AnimationContent.Value.Name);
		        output.WriteObject(l_AnimationContent.Value.Duration);
		        output.WriteObject(l_AnimationContent.Value.Channels.Count);

		        // write each AnimationChannel
                foreach (System.Collections.Generic.KeyValuePair<String, AnimationChannel> l_AnimationChannel in l_AnimationContent.Value.Channels)
		        {
                    output.WriteObject(l_AnimationChannel.Key);
                    output.WriteObject(l_AnimationChannel.Value.Count);

                    int loop;
                    for (loop = 0; loop < l_AnimationChannel.Value.Count; loop++)
                    {
                        output.WriteObject(l_AnimationChannel.Value[loop].Time);
                        output.WriteObject(l_AnimationChannel.Value[loop].Transform);
                    }
		        }
	        }

	        // write the footer
	        output.WriteObject(value.Header3);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(XSIAnimationDataReader).AssemblyQualifiedName;
        }
    }
#endif

    public class XSISASCamera
    {
        public Vector4 NearFarClipping = new Vector4(0, 0, 0, 0);
        public Vector4 Position = new Vector4(0, 0, 0, 0);
    };

    public class XSISASAmbientLight
    {
        public Vector4 Color = new Vector4(0, 0, 0, 0);             
    };

    public class XSISASDirectionalLight
    {
        public Vector4 Color = new Vector4(0, 0, 0, 0);           
        public Vector4 Direction = new Vector4(0, 0, 0, 0);       
    };

    public class XSISASPointLight
    {
        public Vector4 Color = new Vector4(0, 0, 0, 0);
        public Vector4 Position = new Vector4(0, 0, 0, 0);        
        public float Range = 0;                                
    };

    public class XSISASSpotLight
    {
        public Vector4 Color = new Vector4(0, 0, 0, 0);
        public Vector4 Position = new Vector4(0, 0, 0, 0);
        public Vector4 Direction = new Vector4(0, 0, 0, 0);       
        public float Range = 0;                                
        public float Theta = 0;                                
        public float Phi = 0;              
    };

    // SAS utility class
    public class XSISASContainer
    {
        // Matrices
        public Matrix Model;
        public Matrix ModelView;
        public Matrix View;
        public Matrix Projection;
        public Matrix ViewProjection;

	    // scalars
        public float TimeNow;
        public float TimeLast;
        public float TimeFrameNumber;

	    // structures
        public XSISASCamera Camera;
        public List<XSISASAmbientLight> AmbientLights;
        public List<XSISASDirectionalLight> DirectionalLights;
        public List<XSISASPointLight> PointLights;
        public List<XSISASSpotLight> SpotLights;

        public delegate void BindParameter(EffectParameter Parameter);
        public delegate void BindParameterByIndex(EffectParameter Parameter, int index);

        public void BindCameraProjection(EffectParameter Parameter)
        {
            Parameter.SetValue(Projection);
        }

        public void BindCameraPosition(EffectParameter Parameter)
        {
            Parameter.SetValue(Camera.Position);
        }

        public void BindCameraNearFarClipping(EffectParameter Parameter)
        {
            Parameter.SetValue(Camera.NearFarClipping);
        }

        public void BindTimeNow(EffectParameter Parameter)
        {
            Parameter.SetValue(TimeNow);
        }

        public void BindTimeLast(EffectParameter Parameter)
        {
            Parameter.SetValue(TimeLast);
        }

        public void BindTimeFrameNumber(EffectParameter Parameter)
        {
            Parameter.SetValue(TimeFrameNumber);
        }

        public void BindAmbientLightNum(EffectParameter Parameter)
        {
            Parameter.SetValue(AmbientLights.Count);
        }

        public void BindDirectionalLightNum(EffectParameter Parameter)
        {
            Parameter.SetValue(DirectionalLights.Count);
        }

        public void BindPointLightNum(EffectParameter Parameter)
        {
            Parameter.SetValue(PointLights.Count);
        }

        public void BindSpotLightNum(EffectParameter Parameter)
        {
            Parameter.SetValue(SpotLights.Count);
        }

        public void BindAmbientLightColor(EffectParameter Parameter, int index)
        {
            if (index < AmbientLights.Count)
            {
                Parameter.SetValue(AmbientLights[index].Color);
            }
        }

        public void BindDirectionalLightColor(EffectParameter Parameter, int index)
        {
            if (index < DirectionalLights.Count)
            {
                Parameter.SetValue(DirectionalLights[index].Color);
            }
        }

        public void BindDirectionalLightDirection(EffectParameter Parameter, int index)
        {
            if (index < DirectionalLights.Count)
            {
                Parameter.SetValue(DirectionalLights[index].Direction);
            }
        }

        public void BindPointLightColor(EffectParameter Parameter, int index)
        {
            if (index < PointLights.Count)
            {
                Parameter.SetValue(PointLights[index].Color);
            }
        }

        public void BindPointLightPosition(EffectParameter Parameter, int index)
        {
            if (index < PointLights.Count)
            {
                Parameter.SetValue(PointLights[index].Position);
            }
        }

        public void BindPointLightRange(EffectParameter Parameter, int index)
        {
            if (index < PointLights.Count)
            {
                Parameter.SetValue(PointLights[index].Range);
            }
        }

        public void BindSpotLightColor(EffectParameter Parameter, int index)
        {
            if (index < SpotLights.Count)
            {
                Parameter.SetValue(SpotLights[index].Color);
            }
        }

        public void BindSpotLightPosition(EffectParameter Parameter, int index)
        {
            if (index < SpotLights.Count)
            {
                Parameter.SetValue(SpotLights[index].Position);
            }
        }

        public void BindSpotLightDirection(EffectParameter Parameter, int index)
        {
            if (index < SpotLights.Count)
            {
                Parameter.SetValue(SpotLights[index].Direction);
            }
        }

        public void BindSpotLightRange(EffectParameter Parameter, int index)
        {
            if (index < SpotLights.Count)
            {
                Parameter.SetValue(SpotLights[index].Range);
            }
        }

        public void BindSpotLightPhi(EffectParameter Parameter, int index)
        {
            if (index < SpotLights.Count)
            {
                Parameter.SetValue(SpotLights[index].Phi);
            }
        }

        public void BindSpotLightTheta(EffectParameter Parameter, int index)
        {
            if (index < SpotLights.Count)
            {
                Parameter.SetValue(SpotLights[index].Theta);
            }
        }

        // semantic parameter collections
        public Dictionary<String, Matrix>  MatrixParameters;
        public Dictionary<String, BindParameter> VectorOrScalarParameters;
        public Dictionary<String, BindParameterByIndex> ArrayIndexedParameters;

        public XSISASContainer()
        {
            // Create the matrices
            Model = new Matrix(); 
            ModelView = new Matrix();
            View = new Matrix();
            Projection = new Matrix();
            ViewProjection = new Matrix();

            // Initialize
            Model = Matrix.Identity;
            ModelView = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;
            ViewProjection = Matrix.Identity;

            // create the rest
            Camera = new XSISASCamera();
            AmbientLights = new List<XSISASAmbientLight>();
            DirectionalLights = new List<XSISASDirectionalLight>();
            PointLights = new List<XSISASPointLight>();
            SpotLights = new List<XSISASSpotLight>();

            // Create the matrix binding dictionary
            MatrixParameters = new Dictionary<string,Matrix>();
            MatrixParameters.Add("MODEL", Matrix.Identity);
            MatrixParameters.Add("MODELI", Matrix.Identity);
            MatrixParameters.Add("MODELT", Matrix.Identity);
            MatrixParameters.Add("MODELIT", Matrix.Identity);
            MatrixParameters.Add("MODELVIEW", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWI", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWT", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWIT", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWPROJECTION", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWPROJECTIONI", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWPROJECTIONT", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWPROJECTIONIT", Matrix.Identity);
            MatrixParameters.Add("WORLD", Matrix.Identity);
            MatrixParameters.Add("WORLDI", Matrix.Identity);
            MatrixParameters.Add("WORLDT", Matrix.Identity);
            MatrixParameters.Add("WORLDIT", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEW", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWI", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWT", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWIT", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWPROJECTION", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWPROJECTIONI", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWPROJECTIONT", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWPROJECTIONIT", Matrix.Identity);
            MatrixParameters.Add("VIEW", Matrix.Identity);
            MatrixParameters.Add("VIEWI", Matrix.Identity);
            MatrixParameters.Add("VIEWT", Matrix.Identity);
            MatrixParameters.Add("VIEWIT", Matrix.Identity);
            MatrixParameters.Add("PROJECTION", Matrix.Identity);
            MatrixParameters.Add("PROJECTIONI", Matrix.Identity);
            MatrixParameters.Add("PROJECTIONT", Matrix.Identity);
            MatrixParameters.Add("PROJECTIONIT", Matrix.Identity);
            MatrixParameters.Add("MODELINVERSE", Matrix.Identity);
            MatrixParameters.Add("MODELTRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("MODELINVERSETRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWINVERSE", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWTRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWINVERSETRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWPROJECTIONINVERSE", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWPROJECTIONTRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("MODELVIEWPROJECTIONINVERSETRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("WORLDINVERSE", Matrix.Identity);
            MatrixParameters.Add("WORLDTRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("WORLDINVERSETRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWINVERSE", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWTRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWINVERSETRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWPROJECTIONINVERSE", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWPROJECTIONTRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("WORLDVIEWPROJECTIONINVERSETRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("VIEWINVERSE", Matrix.Identity);
            MatrixParameters.Add("VIEWTRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("VIEWINVERSETRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("PROJECTIONINVERSE", Matrix.Identity);
            MatrixParameters.Add("PROJECTIONTRANSPOSE", Matrix.Identity);
            MatrixParameters.Add("PROJECTIONINVERSETRANSPOSE", Matrix.Identity);

            // Create the parameter semantics dictionary
            VectorOrScalarParameters = new Dictionary<string,BindParameter>();
            VectorOrScalarParameters.Add("SAS.CAMERA.PROJECTION", BindCameraProjection);
            VectorOrScalarParameters.Add("SAS.CAMERA.POSITION", BindCameraPosition);
            VectorOrScalarParameters.Add("SAS.CAMERA.NEARFARCLIPPING", BindCameraNearFarClipping);

            VectorOrScalarParameters.Add("SAS.TIME.NOW", BindTimeNow);
            VectorOrScalarParameters.Add("SAS.TIME.LAST", BindTimeLast);
            VectorOrScalarParameters.Add("SAS.TIME.FRAMENUMBER", BindTimeFrameNumber);

            VectorOrScalarParameters.Add("SAS.NUMAMBIENTLIGHTS", BindAmbientLightNum);
            VectorOrScalarParameters.Add("SAS.NUMDIRECTIONALLIGHTS", BindDirectionalLightNum);
            VectorOrScalarParameters.Add("SAS.NUMPOINTLIGHTS", BindPointLightNum);
            VectorOrScalarParameters.Add("SAS.NUMSPOTLIGHTS", BindSpotLightNum);

            ArrayIndexedParameters = new Dictionary<string, BindParameterByIndex>();
            ArrayIndexedParameters.Add("SAS.AMBIENTLIGHTS.COLOR", BindAmbientLightColor);
            ArrayIndexedParameters.Add("SAS.DIRECTIONALLIGHTS.COLOR", BindDirectionalLightColor);
            ArrayIndexedParameters.Add("SAS.DIRECTIONALLIGHTS.DIRECTION", BindDirectionalLightDirection);
            ArrayIndexedParameters.Add("SAS.POINTLIGHTS.COLOR", BindPointLightColor);
            ArrayIndexedParameters.Add("SAS.POINTLIGHTS.POSITION", BindPointLightPosition);
            ArrayIndexedParameters.Add("SAS.POINTLIGHTS.RANGE", BindPointLightRange);
            ArrayIndexedParameters.Add("SAS.SPOTLIGHTS.COLOR", BindSpotLightColor);
            ArrayIndexedParameters.Add("SAS.SPOTLIGHTS.POSITION", BindSpotLightPosition);
            ArrayIndexedParameters.Add("SAS.SPOTLIGHTS.DIRECTION", BindSpotLightDirection);
            ArrayIndexedParameters.Add("SAS.SPOTLIGHTS.RANGE", BindSpotLightRange);
            ArrayIndexedParameters.Add("SAS.SPOTLIGHTS.PHI", BindSpotLightPhi);
            ArrayIndexedParameters.Add("SAS.SPOTLIGHTS.THETA", BindSpotLightTheta);
        }

        public void ComputeViewAndProjection()
        {
            MatrixParameters["VIEW"] = View;
            MatrixParameters["VIEWI"] =  Matrix.Invert(View);
            MatrixParameters["VIEWT"] = Matrix.Transpose(View);
            MatrixParameters["VIEWIT"] = Matrix.Transpose(MatrixParameters["VIEWI"]);
            MatrixParameters["VIEWINVERSE"] = MatrixParameters["VIEWI"];
            MatrixParameters["VIEWTRANSPOSE"] = MatrixParameters["VIEWT"];
            MatrixParameters["VIEWINVERSETRANSPOSE"] = MatrixParameters["VIEWIT"];

            MatrixParameters["PROJECTION"] = Projection;
            MatrixParameters["PROJECTIONI"] = Matrix.Invert(Projection);
            MatrixParameters["PROJECTIONT"] = Matrix.Transpose(Projection);
            MatrixParameters["PROJECTIONIT"] = Matrix.Transpose(MatrixParameters["PROJECTIONI"]);
            MatrixParameters["PROJECTIONINVERSE"] = MatrixParameters["PROJECTIONI"];
            MatrixParameters["PROJECTIONTRANSPOSE"] = MatrixParameters["PROJECTIONT"];
            MatrixParameters["PROJECTIONINVERSETRANSPOSE"] = MatrixParameters["PROJECTIONIT"];

            ViewProjection = View * Projection;
        }

        public void ComputeModel()
        {
            MatrixParameters["MODEL"] = Model;
            MatrixParameters["MODELI"] = Matrix.Invert(Model);
            MatrixParameters["MODELT"] = Matrix.Transpose(Model);
            MatrixParameters["MODELIT"] = Matrix.Transpose(MatrixParameters["MODELI"]);
            MatrixParameters["MODELINVERSE"] = MatrixParameters["MODELI"];
            MatrixParameters["MODELTRANSPOSE"] = MatrixParameters["MODELT"];
            MatrixParameters["MODELINVERSETRANSPOSE"] = MatrixParameters["MODELIT"];
            MatrixParameters["WORLD"] = Model;
            MatrixParameters["WORLDI"] = MatrixParameters["MODELI"];
            MatrixParameters["WORLDT"] = MatrixParameters["MODELT"];
            MatrixParameters["WORLDIT"] = MatrixParameters["MODELIT"];
            MatrixParameters["WORLDINVERSE"] = MatrixParameters["MODELI"];
            MatrixParameters["WORLDTRANSPOSE"] = MatrixParameters["MODELT"];
            MatrixParameters["WORLDINVERSETRANSPOSE"] = MatrixParameters["MODELIT"];

            MatrixParameters["MODELVIEW"] = Model * View;
            MatrixParameters["MODELVIEWI"] = Matrix.Invert(MatrixParameters["MODELVIEW"]);
            MatrixParameters["MODELVIEWT"] = Matrix.Transpose(MatrixParameters["MODELVIEW"]);
            MatrixParameters["MODELVIEWIT"] = Matrix.Transpose(MatrixParameters["MODELVIEWI"]);
            MatrixParameters["MODELVIEWINVERSE"] = MatrixParameters["MODELVIEWI"];
            MatrixParameters["MODELVIEWTRANSPOSE"] = MatrixParameters["MODELVIEWT"];
            MatrixParameters["MODELVIEWINVERSETRANSPOSE"] = MatrixParameters["MODELVIEWIT"];
            MatrixParameters["WORLDVIEW"] = MatrixParameters["MODELVIEW"];
            MatrixParameters["WORLDVIEWI"] = MatrixParameters["MODELVIEWI"];
            MatrixParameters["WORLDVIEWT"] = MatrixParameters["MODELVIEWT"];
            MatrixParameters["WORLDVIEWIT"] = MatrixParameters["MODELVIEWIT"];
            MatrixParameters["WORLDVIEWINVERSE"] = MatrixParameters["MODELVIEWI"];
            MatrixParameters["WORLDVIEWTRANSPOSE"] = MatrixParameters["MODELVIEWT"];
            MatrixParameters["WORLDVIEWINVERSETRANSPOSE"] = MatrixParameters["MODELVIEWIT"];

            MatrixParameters["MODELVIEWPROJECTION"] = Model * ViewProjection;
            MatrixParameters["MODELVIEWPROJECTIONI"] = Matrix.Invert(MatrixParameters["MODELVIEWPROJECTION"]);
            MatrixParameters["MODELVIEWPROJECTIONT"] = Matrix.Transpose(MatrixParameters["MODELVIEWPROJECTION"]);
            MatrixParameters["MODELVIEWPROJECTIONIT"] = Matrix.Transpose(MatrixParameters["MODELVIEWPROJECTIONI"]);
            MatrixParameters["MODELVIEWPROJECTIONINVERSE"] = MatrixParameters["MODELVIEWPROJECTIONI"];
            MatrixParameters["MODELVIEWPROJECTIONTRANSPOSE"] = MatrixParameters["MODELVIEWPROJECTIONT"];
            MatrixParameters["MODELVIEWPROJECTIONINVERSETRANSPOSE"] = MatrixParameters["MODELVIEWPROJECTIONIT"];
            MatrixParameters["WORLDVIEWPROJECTION"] = MatrixParameters["MODELVIEWPROJECTION"];
            MatrixParameters["WORLDVIEWPROJECTIONI"] = MatrixParameters["MODELVIEWPROJECTIONI"];
            MatrixParameters["WORLDVIEWPROJECTIONT"] = MatrixParameters["MODELVIEWPROJECTIONT"];
            MatrixParameters["WORLDVIEWPROJECTIONIT"] = MatrixParameters["MODELVIEWPROJECTIONIT"];
            MatrixParameters["WORLDVIEWPROJECTIONINVERSE"] = MatrixParameters["MODELVIEWPROJECTIONI"];
            MatrixParameters["WORLDVIEWPROJECTIONTRANSPOSE"] = MatrixParameters["MODELVIEWPROJECTIONT"];
            MatrixParameters["WORLDVIEWPROJECTIONINVERSETRANSPOSE"] = MatrixParameters["MODELVIEWPROJECTIONIT"];
        }

        public void SetEffectParameterValue(EffectParameter Parameter)
        {
            if (Parameter.Semantic != null)
            {
                String SemanticName = Parameter.Semantic.ToUpper();
                if (MatrixParameters.ContainsKey(SemanticName))
                {
                    Matrix matrix = MatrixParameters[SemanticName];
                    Parameter.SetValue(MatrixParameters[SemanticName]);
                    return;
                }

                if (VectorOrScalarParameters.ContainsKey(SemanticName))
                {
                    VectorOrScalarParameters[SemanticName](Parameter);
                    return;
                }

                char[] delimiterChars = { '[', ']' };
                string[] splitvalues = SemanticName.Split(delimiterChars);
                if (splitvalues.GetLength(0) == 3)
                {
                    string PartialSemantic = splitvalues[0] + splitvalues[2];
                    int index = System.Convert.ToInt32(splitvalues[1]);
                    if (ArrayIndexedParameters.ContainsKey(PartialSemantic))
                    {
                        ArrayIndexedParameters[PartialSemantic](Parameter, index);
                    }
                }
            }

            if ((Parameter.Annotations["SasBindAddress"] != null) && (Parameter.Annotations["SasBindAddress"].ParameterType == EffectParameterType.String))
            {
                String AnnotationValue = Parameter.Annotations["SasBindAddress"].GetValueString().ToUpper();
                if (MatrixParameters.ContainsKey(AnnotationValue))
                {
                    Parameter.SetValue(MatrixParameters[AnnotationValue]);
                    return;
                }

                if (VectorOrScalarParameters.ContainsKey(AnnotationValue))
                {
                    VectorOrScalarParameters[AnnotationValue](Parameter);
                    return;
                }

                char[] delimiterChars = { '[', ']' };
                string[] splitvalues = AnnotationValue.Split(delimiterChars);
                if (splitvalues.GetLength(0) == 3)
                {
                    string PartialSemantic = splitvalues[0] + splitvalues[2];
                    int index = System.Convert.ToInt32(splitvalues[1]);
                    if (ArrayIndexedParameters.ContainsKey(PartialSemantic))
                    {
                        ArrayIndexedParameters[PartialSemantic](Parameter, index);
                    }
                }
            }

            // check by name only for matrices
            if (MatrixParameters.ContainsKey(Parameter.Name.ToUpper()))
            {
                Parameter.SetValue(MatrixParameters[Parameter.Name.ToUpper()]);
                return;
            }
        }
    };
}