using UnityEngine;
using UnityEngine.Rendering;

public class PortalRenderer : MonoBehaviour {
   private Portal[] portals;

   private void Awake() {
      portals = FindObjectsOfType<Portal>();
      RenderPipelineManager.beginFrameRendering += RenderPortals;
   }

   private void RenderPortals(ScriptableRenderContext context, Camera[] camera) {
   // for (int i = 0; i < portals.Length; i++) {
     //   portals[i].PrePortalRender ();
     // }

      foreach (var portal in portals) {
         portal.Render(context);
      }

      foreach (var portal in portals) {
         portal.PostPortalRender();
      }
   }

   private void OnDestroy() {
      RenderPipelineManager.beginFrameRendering -= RenderPortals;
   }
}
