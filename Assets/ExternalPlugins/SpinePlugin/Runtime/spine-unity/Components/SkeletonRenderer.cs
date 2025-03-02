/******************************************************************************
 * Spine Runtimes Software License v2.5
 *
 * Copyright (c) 2013-2016, Esoteric Software
 * All rights reserved.
 *
 * You are granted a perpetual, non-exclusive, non-sublicensable, and
 * non-transferable license to use, install, execute, and perform the Spine
 * Runtimes software and derivative works solely for personal or internal
 * use. Without the written permission of Esoteric Software (see Section 2 of
 * the Spine Software License Agreement), you may not (a) modify, translate,
 * adapt, or develop new applications using the Spine Runtimes or otherwise
 * create derivative works or improvements of the Spine Runtimes or (b) remove,
 * delete, alter, or obscure any trademarks or any copyright, trademark, patent,
 * or other intellectual property or proprietary rights notices on or in the
 * Software, including any copy thereof. Redistributions in binary or source
 * form must include this license and terms.
 *
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS INTERRUPTION, OR LOSS OF
 * USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
 * IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

#if UNITY_2018_3 || UNITY_2019 || UNITY_2018_3_OR_NEWER
#define NEW_PREFAB_SYSTEM
#endif
#define SPINE_OPTIONAL_RENDEROVERRIDE
#define SPINE_OPTIONAL_MATERIALOVERRIDE

using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity {
	/// <summary>Base class of animated Spine skeleton components. This component manages and renders a skeleton.</summary>
	#if NEW_PREFAB_SYSTEM
	[ExecuteAlways]
	#else
	[ExecuteInEditMode]
	#endif
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)), DisallowMultipleComponent]
	[HelpURL("http://esotericsoftware.com/spine-unity-rendering")]
	public class SkeletonRenderer : MonoBehaviour, ISkeletonComponent, IHasSkeletonDataAsset {
		[SerializeField] public SkeletonDataAsset skeletonDataAsset;

		#region Initialization settings
		/// <summary>Skin name to use when the Skeleton is initialized.</summary>
		[SerializeField] [SpineSkin(defaultAsEmptyString:true)] public string initialSkinName;

		/// <summary>Flip X and Y to use when the Skeleton is initialized.</summary>
		[SerializeField] public bool initialFlipX, initialFlipY;
		#endregion

		#region Advanced Render Settings
		// Submesh Separation
		/// <summary>Slot names used to populate separatorSlots list when the Skeleton is initialized. Changing this after initialization does nothing.</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("submeshSeparators")][SerializeField][SpineSlot] protected string[] separatorSlotNames = new string[0];

		/// <summary>Slots that determine where the render is split. This is used by components such as SkeletonRenderSeparator so that the skeleton can be rendered by two separate renderers on different GameObjects.</summary>
		[System.NonSerialized] public readonly List<Slot> separatorSlots = new List<Slot>();

		// Render Settings
		[Range(-0.1f, 0f)] public float zSpacing;
		/// <summary>Use Spine's clipping feature. If false, ClippingAttachments will be ignored.</summary>
		public bool useClipping = true;

		/// <summary>If true, triangles will not be updated. Enable this as an optimization if the skeleton does not make use of attachment swapping or hiding, or draw order keys. Otherwise, setting this to false may cause errors in rendering.</summary>
		public bool immutableTriangles = false;

		/// <summary>Multiply vertex color RGB with vertex color alpha. Set this to true if the shader used for rendering is a premultiplied alpha shader. Setting this to false disables single-batch additive slots.</summary>
		public bool pmaVertexColors = true;

		/// <summary>Clears the state of the render and skeleton when this component or its GameObject is disabled. This prevents previous state from being retained when it is enabled again. When pooling your skeleton, setting this to true can be helpful.</summary>
		public bool clearStateOnDisable = false;

		/// <summary>If true, second colors on slots will be added to the output Mesh as UV2 and UV3. A special "tint black" shader that interprets UV2 and UV3 as black point colors is required to render this properly.</summary>
		public bool tintBlack = false;

		/// <summary>If true, the renderer assumes the skeleton only requires one Material and one submesh to render. This allows the MeshGenerator to skip checking for changes in Materials. Enable this as an optimization if the skeleton only uses one Material.</summary>
		/// <remarks>This disables SkeletonRenderSeparator functionality.</remarks>
		public bool singleSubmesh = false;

		/// <summary>If true, the mesh generator adds normals to the output mesh. For better performance and reduced memory requirements, use a shader that assumes the desired normal.</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("calculateNormals")] public bool addNormals = false;

		/// <summary>If true, tangents are calculated every frame and added to the Mesh. Enable this when using a shader that uses lighting that requires tangents.</summary>
		public bool calculateTangents = false;

		public string[] SeparatorSlotNames
		{
			get => separatorSlotNames;
			set => separatorSlotNames = value;
		}

		#endregion

		#region Overrides
		#if SPINE_OPTIONAL_RENDEROVERRIDE
		// These are API for anything that wants to take over rendering for a SkeletonRenderer.
		public bool disableRenderingOnOverride = true;
		public delegate void InstructionDelegate (SkeletonRendererInstruction instruction);
		event InstructionDelegate generateMeshOverride;

		/// <summary>Allows separate code to take over rendering for this SkeletonRenderer component. The subscriber is passed a SkeletonRendererInstruction argument to determine how to render a skeleton.</summary>
		public event InstructionDelegate GenerateMeshOverride {
			add {
				generateMeshOverride += value;
				if (disableRenderingOnOverride && generateMeshOverride != null) {
					Initialize(false);
					meshRenderer.enabled = false;
				}
			}
			remove {
				generateMeshOverride -= value;
				if (disableRenderingOnOverride && generateMeshOverride == null) {
					Initialize(false);
					meshRenderer.enabled = true;
				}
			}
		}

		/// <summary> Occurs after the vertex data is populated every frame, before the vertices are pushed into the mesh.</summary>
		public event Spine.Unity.MeshGeneratorDelegate OnPostProcessVertices;
		#endif

		#if SPINE_OPTIONAL_MATERIALOVERRIDE
		[System.NonSerialized] readonly Dictionary<Material, Material> customMaterialOverride = new Dictionary<Material, Material>();
		/// <summary>Use this Dictionary to override a Material with a different Material.</summary>
		public Dictionary<Material, Material> CustomMaterialOverride { get { return customMaterialOverride; } }
		#endif

		[System.NonSerialized] readonly Dictionary<Slot, Material> customSlotMaterials = new Dictionary<Slot, Material>();
		/// <summary>Use this Dictionary to use a different Material to render specific Slots.</summary>
		public Dictionary<Slot, Material> CustomSlotMaterials { get { return customSlotMaterials; } }
		#endregion

		#region Mesh Generator
		[System.NonSerialized] readonly SkeletonRendererInstruction currentInstructions = new SkeletonRendererInstruction();
		readonly MeshGenerator meshGenerator = new MeshGenerator();
		[System.NonSerialized] readonly MeshRendererBuffers rendererBuffers = new MeshRendererBuffers();

        public MeshRenderer MeshRenderer
        {
            get { return meshRenderer; }
        }

        public MeshFilter MeshFilter
        {
            get { return meshFilter; }
        }


        public Color Color
        {
            get
            {
                Color result = Color.white;
                if (skeleton != null)
                {
                    result.a = skeleton.A;
                    result.r = skeleton.R;
                    result.b = skeleton.B;
                    result.g = skeleton.G;
                }
                return result;
            }
            set
            {
                if (skeleton != null)
                {
                    skeleton.A = value.a;
                    skeleton.R = value.r;
                    skeleton.G = value.g;
                    skeleton.B = value.b;
                }
            }
        }

        #endregion

        #region Cached component references
        MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		#endregion

		#region Skeleton
		[System.NonSerialized] public bool valid;
		[System.NonSerialized] public Skeleton skeleton;
		public Skeleton Skeleton {
			get {
				Initialize(false);
				return skeleton;
			}
		}
		#endregion

		public delegate void SkeletonRendererDelegate (SkeletonRenderer skeletonRenderer);

		/// <summary>OnRebuild is raised after the Skeleton is successfully initialized.</summary>
		public event SkeletonRendererDelegate OnRebuild;

		public SkeletonDataAsset SkeletonDataAsset { get { return skeletonDataAsset; } } // ISkeletonComponent

		#region Runtime Instantiation
		public static T NewSpineGameObject<T> (SkeletonDataAsset skeletonDataAsset) where T : SkeletonRenderer {
			return SkeletonRenderer.AddSpineComponent<T>(new GameObject("NewSpineGameObject"), skeletonDataAsset);
		}

		/// <summary>Add and prepare a Spine component that derives from SkeletonRenderer to a GameObject at runtime.</summary>
		/// <typeparam name="T">T should be SkeletonRenderer or any of its derived classes.</typeparam>
		public static T AddSpineComponent<T> (GameObject gameObject, SkeletonDataAsset skeletonDataAsset) where T : SkeletonRenderer {
			var c = gameObject.AddComponent<T>();
			if (skeletonDataAsset != null) {
				c.skeletonDataAsset = skeletonDataAsset;
				c.Initialize(false);
			}
			return c;
		}

		/// <summary>Applies MeshGenerator settings to the SkeletonRenderer and its internal MeshGenerator.</summary>
		public void SetMeshSettings (MeshGenerator.Settings settings) {
			this.calculateTangents = settings.calculateTangents;
			this.immutableTriangles = settings.immutableTriangles;
			this.pmaVertexColors = settings.pmaVertexColors;
			this.tintBlack = settings.tintBlack;
			this.useClipping = settings.useClipping;
			this.zSpacing = settings.zSpacing;

			this.meshGenerator.settings = settings;
		}
		#endregion


		public virtual void Awake () {
			Initialize(false);
		}

        void OnEnable()
        {
            #if SPINE_TK2D
            if (skeletonDataAsset != null)
            {
                if (skeletonDataAsset.spriteCollections != null)
                {
                    for (int i = 0; i < skeletonDataAsset.spriteCollections.Length; i++)
                    {
                        var collection = skeletonDataAsset.spriteCollections[i];
                        if (collection)
                        {
                            collection.AddRetain(this);
                        }
                    }
                }
            }
            Initialize(false);
            #endif
        }


        void OnDisable () {

            #if SPINE_TK2D
            if (skeletonDataAsset != null)
            {
                if (skeletonDataAsset.spriteCollections != null)
                {
                    for (int i = 0; i < skeletonDataAsset.spriteCollections.Length; i++)
                    {
                        var collection = skeletonDataAsset.spriteCollections[i];
                        if (collection)
                        {
                            collection.RemoveRetain(this);
                        }
                    }
                }
            }
            #endif


            if (clearStateOnDisable && valid)
				ClearState();
		}

		void OnDestroy () {
			rendererBuffers.Dispose();
			valid = false;
		}

		/// <summary>
		/// Clears the previously generated mesh and resets the skeleton's pose.</summary>
		public virtual void ClearState () {
			var meshFilter = GetComponent<MeshFilter>();
			if (meshFilter != null) meshFilter.sharedMesh = null;
			currentInstructions.Clear();
			if (skeleton != null) skeleton.SetToSetupPose();
		}

		/// <summary>
		/// Sets a minimum buffer size for the internal MeshGenerator to prevent excess allocations during animation.
		/// </summary>
		public void EnsureMeshGeneratorCapacity (int minimumVertexCount) {
			meshGenerator.EnsureVertexCapacity(minimumVertexCount);
		}

		/// <summary>
		/// Initialize this component. Attempts to load the SkeletonData and creates the internal Skeleton object and buffers.</summary>
		/// <param name="overwrite">If set to <c>true</c>, it will overwrite internal objects if they were already generated. Otherwise, the initialized component will ignore subsequent calls to initialize.</param>
		public virtual void Initialize (bool overwrite) {
			if (valid && !overwrite)
				return;

			// Clear
			{
				if (meshFilter != null)
					meshFilter.sharedMesh = null;

				meshRenderer = GetComponent<MeshRenderer>();
				if (meshRenderer != null) meshRenderer.sharedMaterial = null;

				currentInstructions.Clear();
				rendererBuffers.Clear();
				meshGenerator.Begin();
				skeleton = null;
				valid = false;
			}

			if (skeletonDataAsset == null)
				return;

			SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(false);
			if (skeletonData == null) return;
			valid = true;

			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			rendererBuffers.Initialize();

			skeleton = new Skeleton(skeletonData) {
				scaleX = initialFlipX ? -1 : 1,
				scaleY = initialFlipY ? -1 : 1
			};

            if (!string.IsNullOrEmpty(initialSkinName))
                skeleton.SetInitialSkin(initialSkinName);

            separatorSlots.Clear();
			for (int i = 0; i < separatorSlotNames.Length; i++)
				separatorSlots.Add(skeleton.FindSlot(separatorSlotNames[i]));

			LateUpdate(); // Generate mesh for the first frame it exists.

			if (OnRebuild != null)
				OnRebuild(this);
		}


        public void InitializeMesh()
        {
            this.LateUpdate(); // fill Mesh1
            this.LateUpdate(); // fill Mesh2
        }

        /// <summary>
        /// Generates a new UnityEngine.Mesh from the internal Skeleton.</summary>
        public virtual void LateUpdate () {
			if (!valid) return;

			#if SPINE_OPTIONAL_RENDEROVERRIDE
			bool doMeshOverride = generateMeshOverride != null;
			if ((!meshRenderer.enabled)	&& !doMeshOverride) return;
			#else
			const bool doMeshOverride = false;
			if (!meshRenderer.enabled) return;
			#endif
			var currentInstructions = this.currentInstructions;
			var workingSubmeshInstructions = currentInstructions.submeshInstructions;
			var currentSmartMesh = rendererBuffers.GetNextMesh(); // Double-buffer for performance.

			bool updateTriangles;

			if (this.singleSubmesh) {
				// STEP 1. Determine a SmartMesh.Instruction. Split up instructions into submeshes. =============================================
				MeshGenerator.GenerateSingleSubmeshInstruction(currentInstructions, skeleton, skeletonDataAsset.atlasAssets[0].PrimaryMaterial);

				// STEP 1.9. Post-process workingInstructions. ==================================================================================
				#if SPINE_OPTIONAL_MATERIALOVERRIDE
				if (customMaterialOverride.Count > 0) // isCustomMaterialOverridePopulated 
					MeshGenerator.TryReplaceMaterials(workingSubmeshInstructions, customMaterialOverride);
				#endif

				// STEP 2. Update vertex buffer based on verts from the attachments.  ===========================================================
				meshGenerator.settings = new MeshGenerator.Settings {
					pmaVertexColors = this.pmaVertexColors,
					zSpacing = this.zSpacing,
					useClipping = this.useClipping,
					tintBlack = this.tintBlack,
					calculateTangents = this.calculateTangents,
					addNormals = this.addNormals
				};
				meshGenerator.Begin();
				updateTriangles = SkeletonRendererInstruction.GeometryNotEqual(currentInstructions, currentSmartMesh.instructionUsed);
				if (currentInstructions.hasActiveClipping) {
					meshGenerator.AddSubmesh(workingSubmeshInstructions.Items[0], updateTriangles);
				} else {
					meshGenerator.BuildMeshWithArrays(currentInstructions, updateTriangles);
				}

			} else {
				// STEP 1. Determine a SmartMesh.Instruction. Split up instructions into submeshes. =============================================
				MeshGenerator.GenerateSkeletonRendererInstruction(currentInstructions, skeleton, customSlotMaterials, separatorSlots, doMeshOverride, this.immutableTriangles);

				// STEP 1.9. Post-process workingInstructions. ==================================================================================
				#if SPINE_OPTIONAL_MATERIALOVERRIDE
				if (customMaterialOverride.Count > 0) // isCustomMaterialOverridePopulated 
					MeshGenerator.TryReplaceMaterials(workingSubmeshInstructions, customMaterialOverride);
				#endif

				#if SPINE_OPTIONAL_RENDEROVERRIDE
				if (doMeshOverride) {
					this.generateMeshOverride(currentInstructions);
					if (disableRenderingOnOverride) return;
				}
				#endif

				updateTriangles = SkeletonRendererInstruction.GeometryNotEqual(currentInstructions, currentSmartMesh.instructionUsed);

				// STEP 2. Update vertex buffer based on verts from the attachments.  ===========================================================
				meshGenerator.settings = new MeshGenerator.Settings {
					pmaVertexColors = this.pmaVertexColors,
					zSpacing = this.zSpacing,
					useClipping = this.useClipping,
					tintBlack = this.tintBlack,
					calculateTangents = this.calculateTangents,
					addNormals = this.addNormals
				};
				meshGenerator.Begin();
				if (currentInstructions.hasActiveClipping)
					meshGenerator.BuildMesh(currentInstructions, updateTriangles);
				else
					meshGenerator.BuildMeshWithArrays(currentInstructions, updateTriangles);
			}

			if (OnPostProcessVertices != null) OnPostProcessVertices.Invoke(this.meshGenerator.Buffers);

			// STEP 3. Move the mesh data into a UnityEngine.Mesh ===========================================================================
			var currentMesh = currentSmartMesh.mesh;
			meshGenerator.FillVertexData(currentMesh);

			rendererBuffers.UpdateSharedMaterials(workingSubmeshInstructions);

			if (updateTriangles) { // Check if the triangles should also be updated.
				meshGenerator.FillTriangles(currentMesh);
				meshRenderer.sharedMaterials = rendererBuffers.GetUpdatedSharedMaterialsArray();
			} else if (rendererBuffers.MaterialsChangedInLastUpdate()) {
				meshRenderer.sharedMaterials = rendererBuffers.GetUpdatedSharedMaterialsArray();
			}

			meshGenerator.FillLateVertexData(currentMesh);

			// STEP 4. The UnityEngine.Mesh is ready. Set it as the MeshFilter's mesh. Store the instructions used for that mesh. ===========
			meshFilter.sharedMesh = currentMesh;
			currentSmartMesh.instructionUsed.Set(currentInstructions);
		}

		public void FindAndApplySeparatorSlots (string startsWith, bool clearExistingSeparators = true, bool updateStringArray = false) {
			if (string.IsNullOrEmpty(startsWith)) return;

			FindAndApplySeparatorSlots(
				(slotName) => slotName.StartsWith(startsWith),
				clearExistingSeparators,
				updateStringArray
				);
		}

		public void FindAndApplySeparatorSlots (System.Func<string, bool> slotNamePredicate, bool clearExistingSeparators = true, bool updateStringArray = false) {
			if (slotNamePredicate == null) return;
			if (!valid) return;

			if (clearExistingSeparators)
				separatorSlots.Clear();

			var slots = skeleton.slots;
			foreach (var slot in slots) {
				if (slotNamePredicate.Invoke(slot.data.name))
					separatorSlots.Add(slot);
			}

			if (updateStringArray) {
				var detectedSeparatorNames = new List<string>();
				foreach (var slot in skeleton.slots) {
					string slotName = slot.data.name;
					if (slotNamePredicate.Invoke(slotName))
						detectedSeparatorNames.Add(slotName);
				}
				if (!clearExistingSeparators) {
					string[] originalNames = this.separatorSlotNames;
					foreach (string originalName in originalNames)
						detectedSeparatorNames.Add(originalName);
				}

				this.separatorSlotNames = detectedSeparatorNames.ToArray();
			}

		}

		public void ReapplySeparatorSlotNames () {
			if (!valid)
				return;

			separatorSlots.Clear();
			for (int i = 0, n = separatorSlotNames.Length; i < n; i++) {
				var slot = skeleton.FindSlot(separatorSlotNames[i]);
				if (slot != null) {
					separatorSlots.Add(slot);
				}
				#if UNITY_EDITOR
				else
				{
                    CustomDebug.LogWarning(separatorSlotNames[i] + " is not a slot in " + skeletonDataAsset.skeletonJSON.name);
				}
				#endif
			}
		}

	}
}
