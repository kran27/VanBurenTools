function StringRead f =(
  s = ""
  len = readshort f
  for i = 0 to len - 1 do (
    n = readbyte f
    s += bit.intAsChar n
  )
  return s
)

FormatVer = 0

struct b3d_header (
    Sign			 = "",	--: array[0..7]of char; // "B3D 1.1 "
    Unknown1		 = 0,	--: byte;
    Unknown2		 = 0,	--: byte;
    Unknown3		 = 0,	--: byte; //Header ID??
    GlobalScaleFactor= 0.0,	--: single;
    Unknown4	     = 0,	--: byte; //Header ID??
    CoordinateScale  = 0.0,	--: single;
    Unknown5		 = 0,	--: byte; //Header ID??
    CoordinateSystem = "", --: TCharArray;
    Unknown6		 = 0,	--: byte;
    Unknown7		 = 0,	--: longword;
	function ReadFromFile f =
	(
  	  for i = 0 to 7 do (
        n = readbyte f
        Sign += bit.intAsChar n
      )

	  Unknown2 = readbyte f
	  if Unknown2 == 3 then ( print "a"; FormatVer = 1; for i = 1 to 2 do readbyte f )
	  else ( for i = 1 to 4 do readbyte f )
	  GlobalScaleFactor = readfloat f
	  CoordinateScale = readfloat f
	  CoordinateSystem = StringRead f
	  Unknown6	= readbyte f; Unknown7 = readlong f
	)   
)

Model_Vertex_Position = #()
Model_Vertex_Texcoords = #()
Model_Vertex_Normal   = #()
Model_Faces_Index     = #()
Model_Faces_Mats      = #()

Txs = #()
TexturesDir = ""

Vertex_Type_Flag      = 0
Bones_Flag = 0

FS = 0
Zone_Flag = 0;

struct TVertex64 (
  Coordinate = Point3 0.0 0.0 0.0,      --: array[0..2]of single;
  Normal	 = Point3 0.0 0.0 0.0,      --: array[0..2]of single;
  Color	     = Point4 0.0 0.0 0.0 0.0,  --: array[0..3]of single;
  Unknown1   = Point2 0.0 0.0,          --: array[0..1]of single;
  Unknown2   = Point3 0.0 0.0 0.0,          --: array[0..1]of single;
  Unknown3   = Point2 0.0 0.0,          --: array[0..1]of single;
  function ReadFromFile f = (
    Coordinate.x = readfloat f; Coordinate.z = readfloat f; Coordinate.y = readfloat f
	Model_Vertex_Position[Model_Vertex_Position.count + 1] = Coordinate
	Model_Vertex_Texcoords[Model_Vertex_Texcoords.count +1] = Unknown2
	  
    Normal.x = readfloat f; Normal.y = readfloat f; Normal.z = readfloat f
    Model_Vertex_Normal[Model_Vertex_Normal.count + 1] = Normal	  
	  
	Color.x = readfloat f; Color.y = readfloat f; Color.z = readfloat f; Color.w = readfloat f
	  
	--TexCoords  
	Unknown1.x = readlong f
	for i = 1 to Unknown1.x do
	( 
		if i==1 then
		( 
			Unknown2.x = readfloat f 
			Unknown2.y = 1 - readfloat f
			Model_Vertex_Texcoords[Model_Vertex_Position.count] = Unknown2
		)
		else
		(
			readfloat f
			readfloat f
		)
	)
	Unknown1.x = readlong f
	for i = 1 to Unknown1.x do (readfloat f; readfloat f;)
) 
  
)

struct TVertex44 (
  Coordinate   = Point3 0.0 0.0 0.0, -- Eii?aeiaoa oi?ee (x, y, z: single)
  Normal	   = Point3 0.0 0.0 0.0, -- Ii?iaeu oi?ee (x, y, z: single)
  Color	       = #(0, 0, 0, 0),      -- Oaao oi?ee (r, g, b, a: byte)
  TextureCoord = Point3 0.0 0.0 0.0,     -- Oaenoo?iua eii?aeiaou (s, t: single);
  Unknown1     = #(),     -- Iaecaanoii
  function ReadFromFile f = (
    Coordinate.x = readfloat f; Coordinate.z = readfloat f; Coordinate.y = readfloat f
	Model_Vertex_Position[Model_Vertex_Position.count + 1] = Coordinate
	  
    Normal.x = readfloat f; Normal.y = readfloat f; Normal.z = readfloat f
	Model_Vertex_Normal[Model_Vertex_Normal.count + 1] = Normal	 
	  
    for i = 1 to 4 do Color[i] = readbyte f
		
	TextureCoord.x = readfloat f; TextureCoord.y = 1 - readfloat f
	Model_Vertex_Texcoords[Model_Vertex_Texcoords.count + 1] = TextureCoord
	
    Unknown1[1] = readlong f; Unknown1[2] = readlong f
  ) 
)

struct TVert_Bones_Data (
	Bone   = 0,   --: longword
	Weight = 0.0, --: single
	function ReadFromFile f = (
	  Bone = readlong f
	  Weight = readfloat f
	)
)

struct TMaterial_Data44 (
    Name		  = "",		
    Mtl_ID		  = "",	   
    Unknown1	  = 0.0, 
    BLEND_STATE   = "",	
    MATERIAL_TYPE = "", 
    Unknown2	  = 0, 
    Zone		  = "", 
    Unknown3	  = 0, 
	function ReadFromFile f = (
      Name = StringRead f; Mtl_ID = StringRead f
      for i = 0 to 16 do Unknown1 = readfloat f
      BLEND_STATE = StringRead f; MATERIAL_TYPE = StringRead f
	  a = readlong f; print a
      for i = 1 to 16 do ( Unknown2 = readlong f; if Unknown2 == 1 then 
		  (
			  Zone = StringRead f;
			  tm = bitmaptexture()
			  tm.alphasource = 2
			  tm.filename = TexturesDir + Zone
			  Txs[Txs.count + 1] = standardMaterial diffuseMap:tm showInViewport:true
			  
			  Unknown2 = readlong f;
	          for i = 1 to Unknown2 do StringRead f;
			  exit;
		  ) 
		)
	)
)

struct TMaterial_Data (
    Name		  = "",	--: TCharArray;
    Mtl_ID		  = "",	--: TCharArray;
    Unknown1	  = 0.0, --: array[0..16]of single; //Most likely Ambient, Diffuse, Emissive, Specular, Shininess, and Alpha
    BLEND_STATE   = "",	--: TCharArray;
    MATERIAL_TYPE = "", --: TCharArray;
    Unknown2	  = 0, --: array[0..63]of byte; //Flags??
    Zone		  = "", --: TCharArray;
    Unknown3	  = 0, --: longword;
	function ReadFromFile f = (
	Unknown3 = readlong f
		
      Name = StringRead f
      a = readlong f;  for i = 1 to a do Mtl_ID = StringRead f
	
	  tm = bitmaptexture()
	  tm.alphasource = 2
	  if Mtl_ID != "" then tm.filename = TexturesDir +  Mtl_ID
	  Txs[Txs.count + 1] = standardMaterial diffuseMap:tm showInViewport:true
	  
		local kk = 0
	  readbyte f; readlong f; kk = readlong f; print kk
      for i = 1 to kk/3 do (
		  a = 0; a = readlong f
		  b = 0; b = readlong f
		  c = 0; c = readlong f
		  a = a + 1;  b = b + 1; c = c + 1;
		  Model_Faces_Index[Model_Faces_Index.count + 1] = [b, a, c]	
		  Model_Faces_Mats[Model_Faces_Mats.count + 1] = Txs.count ;		  
		  )
      for i = 1 to kk do (Unknown2 = readbyte f)
	)
)


struct TVertex_Node (
    Unknown1       = 0,		
	Unknown2       = 0,		
	VertexTypeFlag = 0, 	
	NumberVerts    = 0,		
	SizeOfVerts    = 0,		
    Unknown3       = #(),	
	IsBones       = 0,     
	BonesPerVert   = 0,		
	NumberMaterial = 0,		
    function ReadFromFile f = (
      Unknown1 = readbyte f; 
	  Unknown2 = readbyte f;
	  format "VertNode U1 % U2 %\n" Unknown1 Unknown2;
		
	  if FormatVer == 0 then
	  (
	  VertexTypeFlag = readbyte f;
	  Vertex_Type_Flag = VertexTypeFlag
	  )
	  else Vertex_Type_Flag = 0
	  
	  NumberVerts = readlong f;
	  format "Vertex count - %\n" NumberVerts
		format "Vertex_Type_Flag - %\n" Vertex_Type_Flag
      if Vertex_Type_Flag == 1 then (
	    SizeOfVerts = readlong f;
		format "SizeOfVerts - %\n" SizeOfVerts
		for i = 1 to NumberVerts do (Vertex44 = TVertex44(); Vertex44.ReadFromFile f)
		for i = 1 to 6 do (Unknown3[i] = readfloat f); IsBones = readbyte f

		 if IsBones == 1 then
		 (
			BonesPerVert = readlong f
			format "BonesPerVert - %\n" BonesPerVert
			for i = 1 to NumberVerts * BonesPerVert do (VBD = TVert_Bones_Data(); VBD.ReadFromFile f;)
		 )
		  
		NumberMaterial = readlong f
		format "NumberMaterial - %\n" NumberMaterial
		for i = 1 to NumberMaterial do (MD = TMaterial_Data44(); MD.ReadFromFile f; print MD)
		
		 --for i = 1 to NumberMaterial do (
		 -- Zone = ""; S = ""
		 -- for j = 1 to Zone_Flag + 1 do (
		 --   S = StringRead f; Zone += S
		 -- )
		 -- print Zone
		 -- pos = ftell f; print Pos
		--)
		 
		 --StringRead f

		 for i = 1 to NumberMaterial do (StringRead f)
		 --StringRead f		 
			
		 Mat_list = #()
		 
		NumVert = 0; NumTri = 0
		for i = 1 to NumberMaterial do (a = readlong f; Mat_list[Mat_list.count+1] = a )
		for i = 1 to NumberMaterial do (a = readlong f; NumVert +=a; )
		print "NumVert"; print NumVert; print "NumTri"; print NumTri;
		NumTri = NumTri / 3;
		for j = 1 to NumberMaterial do (
		for i = 1 to (Mat_list[j]/3)  do (
		  a = 0; a = readshort f #unsigned;
		  b = 0; b = readshort f #unsigned;
		  c = 0; c = readshort f #unsigned;
		  a = a + 1;  b = b + 1; c = c + 1;
		  Model_Faces_Index[Model_Faces_Index.count + 1] = [c, b, a];
		  Model_Faces_Mats[Model_Faces_Mats.count + 1] = j;
			--print Model_Faces_Index[Model_Faces_Index.count] 
		))
		--k = readlong f;
		--for i = 1 to k do ( l = StringRead f; print l; )
	  ) else (
			
		for i = 1 to NumberVerts do (Vertex64 = TVertex64(); Vertex64.ReadFromFile f)
		pos = ftell f; print pos
		NumberMaterial = readlong f
		format "NumberMaterial - %\n" NumberMaterial
		for i = 1 to NumberMaterial do (MD = TMaterial_Data(); MD.ReadFromFile f; print MD)
		--k = readlong f;
		--for i = 1 to k do ( l = StringRead f; print l; )
	  )
--	  Print Model_Vertex_Position
    )
)

struct TBone (
    Name	    = "",
    Unknown1	= 0, --: word;
    Flag  	    = 0, --: byte;  //Header ID?? For "Transform"
    Translation	= point3 0.0 0.0 0.0, --: array[0..2]of single;
    Rotation	= quat 0.0 0.0 0.0 0.0, --: array[0..3]of single;
	function ReadFromFile f = (
	Name = StringRead f; Unknown1 = readshort f; Flag = readbyte f
	if Flag != 0 then 
	(
		Translation.x = readfloat f; Translation.z = readfloat f; Translation.y = readfloat f;
		if Flag == 3 then (Rotation.x = readfloat f; Rotation.z = readfloat f; Rotation.y = readfloat f; Rotation.w = readfloat f)
		if Flag == 2 then readfloat f
	)
	)
)

struct TColor (
  r = 0, g = 0, b = 0, a = 0,
  function ReadFromFile f = (r = readfloat f; g = readfloat f; b = readfloat f; a = readfloat f
  )
)

struct TMater (
  Ambient   = TColor(),
  Diffuse   = TColor(),
  Emissive  = TColor(),
  Specular  = TColor(),
  Shininess = 0,
  Alpha     = 0,
  function ReadFromFile f = (
    Ambient.ReadFromFile f; Diffuse.ReadFromFile f
    Emissive.ReadFromFile f; Specular.ReadFromFile f
    Shininess = readfloat f; Alpha = readfloat f
  )
)

struct TMaterials (
    Mtl_ID = "",
    Name = "",
    Mater = TMater(),
    BLEND_STATE = "",
    MATERIAL_TYPE = "",
    Flag_1 = 0,
	Flag_2 = 0,
	function ReadFromFile f = (
      Mtl_ID = StringRead f; Name = StringRead f
      Mater.ReadFromFile f
      BLEND_STATE = StringRead f; MATERIAL_TYPE = StringRead f
      Flag_1 = readlong f; Flag_2 = readlong f
	)
)

struct TTextures (
    Unknown1 = 0, --: byte;
    Name = "", --: TCharArray;
    FileName = "", --: TCharArray;
    Width = 0, --: longword;
    Height = 0, --: longword;
	function ReadFromFile f = (
      Unknown1 = readbyte f; Name = StringRead f; FileName = StringRead f
      Width = readlong f; Height = readlong f
	)
)

struct TNodes (
  Unknown1 = 0, --: byte;
  Name = "", --: TCharArray;
  function ReadFromFile f = (
    Unknown1 = readbyte f; Name = StringRead f
  )
)

struct TMat_Data (
    Unknown1 = 0, --: array[0..2]of byte;
    Name     = "", --: TCharArray;
    TextureOn = 0, --: longword;
    Texture  = "", --: TCharArray;
    Unknown3 = 0, --: longword;
    Unknown4 = 0, --: byte;
    NumOfPoints = 0, --: longword;
    function ReadFromFile f = (
      for i = 0 to 2 do (Unknown1 = readbyte f)
	  Name = StringRead f
	  print Name
	  TextureOn = readlong f
	  if TextureOn != 0 then Texture = StringRead f
	  Unknown3 = readlong f
	  Unknown4 = readbyte f
	  NumOfPoints = readlong f
      print NumOfPoints
    )
	
)
--   Points                      : array of longword;
--    MatID                       : array of byte;
struct TMaterial_Node (
    NumberMaterialNodes	= 0, --: longword;
    Unknown1            = 0, --: byte;
    function ReadFromFile f = (
 	  NumberMaterialNodes = readlong f
	  Unknown4 = readbyte f
      format "MaterialNode - NumberMaterialNodes = %d\n"  NumberMaterialNodes 
    )
 --   Mat_Data                    : array of TMat_Data;
)

function FileSizeGet f = (
  fseek f 0 #seek_end
  size = ftell f
  return size
)

function FilePosGet f = (
  pos = ftell f
  return pos
)

function main f asmooth dir = (
	print f
	TexturesDir = dir
  local FileSize = 0
  local FilePos = 0
  FileSize = FileSizeGet f
  FS = FileSize
  fseek f 0 #seek_set
  b3d_head = b3d_header(); b3d_head.ReadFromFile f; print "head"; print b3d_head; print "head_end"
  Material_Node = TMaterial_Node()
  do (
    b = 0; b = readbyte f
	case b of (
      0x07: (Materials = TMaterials(); Materials.ReadFromFile f; print "Materials"; print Materials; pos = ftell f; print pos)
	  0x08: (Textures = TTextures(); Textures.ReadFromFile f; print "Textures"; print Textures; pos = ftell f; print pos)
	  0x0A: (Nodes = TNodes(); Nodes.ReadFromFile f; print "Nodes";  print Nodes; pos = ftell f; print pos)
	  0x0E: (Bones_Flag = 1; Bone1 = TBone(); Bone1.ReadFromFile f; print "Bones"; print Bone1; pos = ftell f; print pos)
	  0x0F: (Vertex_Node = TVertex_Node(); Vertex_Node.ReadFromFile f; print "VertexNodes"; pos = ftell f; print pos)
	  default: exit
	) 
	FilePos = FilePosGet f
  )
  while FilePos != FileSize

  print (FilePosGet f);
  
  local obj
  obj = Editable_Mesh()
  
  obj.material = multimaterial numsubs:Txs.count
  
  print "tx count"; print Txs.count
  for i = 1 to Txs.count do
  (
	  obj.material.materialList[i] = Txs[i]
	  meditMaterials[i] = Txs[i]
  )
  
  obj.mesh.numverts = Model_Vertex_Position.count
  obj.mesh.numfaces = Model_Faces_Index.count
  obj.mesh.numtverts = Model_Vertex_Position.count
  
  format "tx count %\n" Model_Vertex_Texcoords.count
  format "vert count %\n" Model_Vertex_Position.count
  format "faces count %\n" Model_Faces_Index.count
  
  for i = 1 to obj.mesh.numverts do
	  SetVert obj.mesh i Model_Vertex_Position[i]
  
  for i = 1 to obj.mesh.numfaces do
	  SetFace obj.mesh i Model_Faces_Index[i]
  
  for i = 1 to obj.mesh.numtverts do
	  SetTVert obj.mesh i Model_Vertex_Texcoords[i]
  
  buildTVFaces obj.mesh false
  
  for i = 1 to obj.mesh.numfaces do
  (
	  setTVFace obj.mesh i Model_Faces_Index[i]
	  setFaceMatID obj.mesh i Model_Faces_Mats[i]
  )
  
  
  meshop.weldVertsByThreshold obj.mesh #{1..obj.mesh.numverts} 0.001
  meshop.autoSmooth obj.mesh #{1..obj.mesh.numfaces} asmooth
  
  addmodifier obj (Unwrap_UVW())
   update obj 
)

m_roll=newrolloutfloater "Van Buren Multi Importer" 400 241

rollout b3dImport ".B3D File Import" width:400 height:104
(
	edittext filename "File:" pos:[8,8] width:312 height:17
	button OpenBtn "Open" pos:[326,7] width:56 height:21 enabled:true
	button ImportBtn "Import" pos:[8,72] width:80 height:21
	spinner asmooth "Auto Smooth: " pos:[32,39] width:160 height:16 range:[0,180,0] scale:0.1 
	
	
	
	on b3dImport open do
	(
		ASmooth.value = 45.0
	)
	on OpenBtn pressed do
	(
		s = GetOpenFileName types:"B3D file(*.b3d)|*.b3d" historyCategory:"b3dImports"
		if s != undefined do
		(
			filename.text = s
		)	
	)
	on ImportBtn pressed do
	(
		if FileName.text != "" do
		(
			main (fopen FileName.text "rb") ASmooth.value (getFilenamePath FileName.text)
			fclose FileName	
			closeRolloutFloater m_roll			
		)
	)
)

addrollout b3dImport m_roll

global MapName = "defaultName"
function String4Read f = (
	s = ""
	for i = 1 to 4 do (
		n = readbyte f
		s+= bit.intAsChar n
	)
	return s
)

struct TSectionHeader
(
	id = "", -- name
	size = 0,
	
	function ReadFromFile f =
	(
		id = String4Read f
		size = readlong f
	)
)

function ReadSectionHead f =
(
	head = TSectionHeader()
	head.ReadFromFile f
	return head
)


GVP = #() -- vertex position
GVN = #() -- vertex normal
GVD = #() -- vertex diffuse color
GVT = #() -- texture coords
GVL = #() -- lightmap coords

IDX = #() -- face indexes
MDX = #() -- mat index

MAT = #()

buffer_index = 1  -- current vertex buffer index
buffer_offset = #() -- 

Flag = 1
dir = ""

struct TREE_Data
(
	
	function FileSizeGet f = 
	(
		prev = ftell f
		fseek f 0 #seek_end
		size = ftell f
		fseek f prev #seek_set
		return size
	),
	
	function FilePosGet f =
	(
		ret = ftell f
		return ret
	),
	
	function IsHeader s =
	(
		return case s of
		(
			"NODE" 	: true
			"LEAF" 	: true
			"INFO"	: true
			"IDXD"	: true
			"IDXS"	: true
			"FLGS"	: true
			"TREE"	: true
			"LVL0"	: true
			"XYZ "	: true
			"MATR"	: true
			"MATD"	: true
			"TXTD"	: true
			"TREE"	: true
			"VTXD"	: true
			"VTXB"	: true
			"TXUV"	: true
			"LMUV"	: true
			"DIFU"	: true
			"NRML"	: true
			"LVLD"	: true
			"LVL0"	: true
			"HEAD"	: true
			"8TRE"	: true
			"LGTD"	: true
			"LGTR"	: true
			default	: false
		)
	),

	function ReadBlock f size= 
	(
		header = ReadSectionHead f
		
	    if header.id == "FLGS" do
		(			
			fseek f (header.size*Flag) #seek_cur
			return 0
		)
		
		if header.id == "IDXD" do
		(
			fseek f -16 #seek_cur
			tmp =1 + (readlong f)
			if tmp >= 1 and tmp <= buffer_offset.count do 
				buffer_index = tmp
			--format "vertex buffer : % / %" buffer_index buffer_offset.count
			fseek f 12 #seek_cur
		)
		
		if header.id == "TXTD" do
		(
			print (readlong f)
			print (readlong f)
		)
		
		if header.id == "TXUV" do
		(
			for i = 1 to header.size/8 do
			(
				u = readfloat f
				v = readfloat f
				
				GVT[GVT.count+1] = [u,1-v,0]
			)
		)
		
		if header.id == "TXTR" do
		(
			s = ""
			for i = 1 to header.size do s += (bit.IntAsChar (readbyte f) )
			--print s

			if (FindString s "ctx") != undefined do  return 0 
			
			tm = bitmaptexture()
			tm.alphasource = 2
			tm.filename = dir+"../Tiles/" + s
			print s
			MAT[MAT.count + 1] = standardMaterial diffuseMap:tm showInViewport:true
			
			return 0
		)
		
		if header.id == "IDXS" do
		(
			--print header
			end = (ftell f) + header.size
			m = 0
			mp = 0
		
			while ((ftell f)+12) < end do
			(
				tmp = readshort f #unsigned
				if tmp == 0 do
				(
					tmp = readshort f #unsigned
					--print tmp
				)
				
				if bit.and tmp 0xFF00 == 0x8000 do
				(
					/*fseek f 8 #seek_cur;*/ 
					readshort f
					t1 = (readshort f) - 0x4000
					readshort f
					t2 = readshort f
					m = t2
					--format "80 - % 40 - % - %\n" (tmp-0x8000) t1 t2
				)
				
				if bit.and tmp 0xFF00 == 0x4000 do
				(
					--fseek f 4 #seek_cur
					readshort f
					t1 = readshort f
					m = t1
					--format "40 - % - %\n" (tmp-0x4000) t1 
				)
				
				if bit.and tmp 0xFF00 == 0 do  m = tmp
					
				size = readshort f
				
				for i = 1 to (size/3) do
				(
					a = buffer_offset[buffer_index] + readshort f #unsigned
					b = buffer_offset[buffer_index] + readshort f #unsigned
					c = buffer_offset[buffer_index] + readshort f #unsigned
					
					IDX[IDX.count+1] = [a,c,b]
					MDX[MDX.count+1] = if m == 0 then 0 else m
				)
				
			)
			
			fseek f end #seek_set			
			return 0
		)
		
		if header.id == "VTXB" do
		(
			readlong f
			header.size -= 4
			buffer_offset[buffer_offset.count+1] = GVP.count+1
			--format "vertex buffer offset % = %\n" buffer_offset.count  (GVP.count+1)
		)
		
		if header.id == "VTXD" do
		(
			readlong f
			header.size -= 4 
		)
		
		if header.id == "XYZ " do
		(
			--print header
			
			for i = 1 to header.size/12 do
			(
				x = readfloat f
				y = readfloat f
				z = readfloat f
				
				GVP[GVP.count+1] = [x,z,y]
			)
			
			return 0
		)
		
		if IsHeader header.id then
		(
			--print header
			
			end = ( ftell f ) + header.size
			
			while (ftell f ) < end do
			(				
				ReadBlock f header.size
			)
		)
		else
		(
			fseek f -8 #seek_cur
			if size < 4  and size != 0 do size = 4
			fseek f size #seek_cur
		)
	),

	
	function ReadFromFile f =
	(
		ReadBlock f 0
	)
)


function tre_file f  = 
(	
	
	reader = TREE_Data()
	reader.ReadFromFile f
	
	obj = Editable_Mesh()
	obj.name = getFilenameFile MapName
	obj.mesh.numverts = GVP.count
	obj.mesh.numtverts = GVT.count
	obj.mesh.numfaces = IDX.count
	
	obj.material = multimaterial numsubs:MAT.count
  
	format "mats = %\n" MAT.count
	for i = 1 to MAT.count do
	(
		obj.material.materialList[i] = MAT[i]
		meditMaterials[i] = MAT[i]
	)
	
	for i = 1 to obj.mesh.numverts do
	  SetVert obj.mesh i GVP[i]
  
	for i = 1 to obj.mesh.numfaces do
		SetFace obj.mesh i IDX[i]
	
	for i = 1 to obj.mesh.numtverts do
		SetTVert obj.mesh i GVT[i]
	  
	buildTVFaces obj.mesh false
	
	for i = 1 to obj.mesh.numfaces do
	(
		setTVFace obj.mesh i IDX[i]
		setFaceMatID obj.mesh i MDX[i]
	)
	
	addmodifier obj (Unwrap_UVW())	
	update obj
)

rollout MapImport ".8 File Import" width:400 height:104
(
	edittext filename "File:" pos:[9,6] width:312 height:17
	button OpenBtn "Open" pos:[326,7] width:56 height:21 enabled:true
	button ImportBtn "Import" pos:[9,68] width:80 height:21
	checkbox Flgs "FLGS Size" pos:[9,39] width:94 height:12
	
	on OpenBtn pressed do
	(
		s = GetOpenFileName types:".8 file(*.8)|*.8" historyCategory:"MapImports"
		if s != undefined do
		(
			filename.text = s
			MapName = s
		)	
	)
	on ImportBtn pressed do
	(
		if FileName.text != "" do
		(
			Flag = if Flgs.checked then 4 else 1
			dir = getFilenamePath FileName.text
			tre_file (fopen FileName.text "rb")
			fclose FileName	
			closeRolloutFloater m_roll			
		)
	)
)

addrollout MapImport m_roll