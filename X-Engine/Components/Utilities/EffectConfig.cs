using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
  public class SASCamera
  {
      public Vector4 NearFarClipping = new Vector4(0, 0, 0, 0);
      public Vector4 Position = new Vector4(0, 0, 0, 0);
  };

  public class SASAmbientLight
  {
      public Vector4 Color;

      public SASAmbientLight()
      { Color = new Vector4(0, 0, 0, 0); }
      public SASAmbientLight(Vector4 color)
      { Color = color; }
      public SASAmbientLight(float r, float g, float b, float w)
      { Color = new Vector4(r, g, b, w); }
  }


  public class SASDirectionalLight
  {
      public Vector4 Color;
      public Vector4 Direction;

      public SASDirectionalLight()
      {
          Color = new Vector4(0, 0, 0, 0);
          Direction = new Vector4(0, 0, 0, 0);
      }

      public SASDirectionalLight(Vector4 color, Vector4 direction)
      { Color = color; Direction = direction; }
      public SASDirectionalLight(float cr, float cg, float cb, float cw, float dx, float dy, float dz, float dw)
      { Color = new Vector4(cr, cg, cb, cw); Direction = new Vector4(dx, dy, dz, dw); }
  }

  public class SASPointLight
  {
      public Vector4 Color = new Vector4(0, 0, 0, 0);
      public Vector4 Position = new Vector4(0, 0, 0, 0);
      public float Range = 0;
      public SASPointLight() { }
      public SASPointLight(Vector4 color, Vector4 position, float range)
      { Color = color; Position = position; Range = range; }
      public SASPointLight(float cr, float cg, float cb, float cw, float px, float py, float pz, float pw, float range)
      { Color = new Vector4(cr, cg, cb, cw); Position = new Vector4(px, py, pz, pw); Range = range; }
  }

  public class SASSpotLight
  {
      public Vector4 Color = new Vector4(0, 0, 0, 0);
      public Vector4 Position = new Vector4(0, 0, 0, 0);
      public Vector4 Direction = new Vector4(0, 0, 0, 0);
      public float Range = 0;
      public float Theta = 0;
      public float Phi = 0;
  };

  // SAS utility class
  public class SASContainer
  {
      //Ref to XMain
      private XMain X;

      // Matrices
      public Matrix World;
      public Matrix WorldView;
      public Matrix View;
      public Matrix Projection;

      // scalars
      public float TimeNow;
      public float TimeLast;
      public float TimeFrameNumber;

      //Animation Specific
      public Vector4[] Pose;

      // structures
      public SASCamera Camera;
      public List<SASAmbientLight> AmbientLights;
      public List<SASDirectionalLight> DirectionalLights;
      public List<SASPointLight> PointLights;
      public List<SASSpotLight> SpotLights;

      public delegate void BindParameter(EffectParameter Parameter);
      public delegate void BindParameterByIndex(EffectParameter Parameter, int index);

      public void BindEnvironment()
      {
          //Bind to shader

          //Ambient environment
          SASAmbientLight EnvironAmbient = new SASAmbientLight(X.Environment.LightColorAmbient);
          SASDirectionalLight EnvronSun = new SASDirectionalLight(X.Environment.LightColor, X.Environment.LightDirection);
          AmbientLights.Add(EnvironAmbient);
          DirectionalLights.Add(EnvronSun);

          //Shadows
          //Fog
          
      }

      public void DefaultLighting()
      {
          // initialize lights by default
          SASPointLight light1 = new SASPointLight();
          SASPointLight light2 = new SASPointLight();
          SASPointLight light3 = new SASPointLight();

          light1.Color = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
          light2.Color = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
          light3.Color = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);

          light1.Position = new Vector4(100.0f, 100.0f, 100.0f, 1.0f);
          light2.Position = new Vector4(-100.0f, 100.0f, 100.0f, 1.0f);
          light3.Position = new Vector4(0.0f, 0.0f, -100.0f, 1.0f);

          light1.Range = 10000.0f;
          light2.Range = 10000.0f;
          light3.Range = 10000.0f;

          PointLights.Add(light1);
          PointLights.Add(light2);
          PointLights.Add(light3);
      }

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
              try
              {
                  //Try set to a vector4 color
                  Parameter.SetValue(AmbientLights[index].Color);
              }
              catch (Exception e)
              {
                  //Vector4 failed try a vector3 instead
                  Parameter.SetValue(new Vector3(AmbientLights[index].Color.X, AmbientLights[index].Color.Y, AmbientLights[index].Color.Z));
              }
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
      public Dictionary<String, Matrix> MatrixParameters;
      public Dictionary<String, BindParameter> VectorOrScalarParameters;
      public Dictionary<String, BindParameterByIndex> ArrayIndexedParameters;
      public Dictionary<String, Vector4[]> ArrayParameters;

      public SASContainer(ref XMain X)
      {
          //save ref to XMain
          this.X = X;
          
          // Create the matrices
          World = new Matrix();
          WorldView = new Matrix();
          View = new Matrix();
          Projection = new Matrix();

          // Custom Parameters
          Pose = new Vector4[0];

          // Initialize
          World = Matrix.Identity;
          WorldView = Matrix.Identity;
          View = Matrix.Identity;
          Projection = Matrix.Identity;

          // create the rest
          Camera = new SASCamera();
          AmbientLights = new List<SASAmbientLight>();
          DirectionalLights = new List<SASDirectionalLight>();
          PointLights = new List<SASPointLight>();
          SpotLights = new List<SASSpotLight>();

          // Create the matrix binding dictionary
          MatrixParameters = new Dictionary<string, Matrix>();
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
          MatrixParameters.Add("VIEWPROJECTION", Matrix.Identity);
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
          VectorOrScalarParameters = new Dictionary<string, BindParameter>();
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

          ArrayParameters = new Dictionary<string, Vector4[]>();
          ArrayParameters.Add("SKINPOSE", Pose);
      }

      public void ComputeViewAndProjection()
      {
          MatrixParameters["VIEW"] = View;
          MatrixParameters["VIEWPROJECTION"] = View * Projection;
          MatrixParameters["VIEWI"] = Matrix.Invert(View);
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

          
      }

      public void ComputeModel()
      {
          //Animation data specific
          ArrayParameters["SKINPOSE"] = Pose;
          MatrixParameters["MODEL"] = World;
          MatrixParameters["MODELI"] = Matrix.Invert(World);
          MatrixParameters["MODELT"] = Matrix.Transpose(World);
          MatrixParameters["MODELIT"] = Matrix.Transpose(MatrixParameters["MODELI"]);
          MatrixParameters["MODELINVERSE"] = MatrixParameters["MODELI"];
          MatrixParameters["MODELTRANSPOSE"] = MatrixParameters["MODELT"];
          MatrixParameters["MODELINVERSETRANSPOSE"] = MatrixParameters["MODELIT"];
          MatrixParameters["WORLD"] = World;
          MatrixParameters["WORLDI"] = MatrixParameters["MODELI"];
          MatrixParameters["WORLDT"] = MatrixParameters["MODELT"];
          MatrixParameters["WORLDIT"] = MatrixParameters["MODELIT"];
          MatrixParameters["WORLDINVERSE"] = MatrixParameters["MODELI"];
          MatrixParameters["WORLDTRANSPOSE"] = MatrixParameters["MODELT"];
          MatrixParameters["WORLDINVERSETRANSPOSE"] = MatrixParameters["MODELIT"];

          MatrixParameters["MODELVIEW"] = World * View;
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

          MatrixParameters["MODELVIEWPROJECTION"] = World * View* Projection;
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

      //Rendercode calls this for each effect parameter
      //we need to pick and chose which values from this class get set
      public void SetEffectParameterValue(EffectParameter Parameter)
      {
          //if(Parameter.ParameterType == EffectParameterType.)
          if (Parameter.Semantic != null)
          {
              if (Parameter.ParameterType == EffectParameterType.Single || Parameter.ParameterType == EffectParameterType.Int32 || Parameter.ParameterType == EffectParameterType.Bool)
              {
                  String SemanticName = Parameter.Semantic.ToUpper();
                  if (ArrayParameters.ContainsKey(SemanticName))
                  {
                      //Assign scalar array
                      Parameter.SetValue(ArrayParameters[SemanticName]);
                      return;
                  }
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

                  char[] delimiterChars = {'[', ']'};
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
