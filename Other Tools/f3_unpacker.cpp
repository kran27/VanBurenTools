#include <stdio.h>
#include <direct.h>

struct F3RHTHeader
{
    long vMajor;
    long vMinor;
    long nEntries;//number of file entries
	long offsetPacks;//offset to PACK file data
	long offsetResources;//offset to file name strings
};

enum ftype
{
	G3D=100,// 3d model
	TGA=200,//targa image
	SMA=300,//script file (i presume the original extension was .sma)
	TRE=400,//unknown file header is 8TRE, so extension is .tre :)
	SKEL=600,//3d skeleton
	ANIM=700,//3d skeleton + animation
	INI=800,//text file
	WAV=1100,//RIFF sound wave file
	SCR=1200,//script i am assuming that the extension is .scr
	MAP=1300,//map
	CRITTER_ENT=1400,//entity data/critter
	VFX=1500,//vertex/pixel shaders + effects
	GUI=1600,//Graphical User Interface
	COL=1700,//i presume that this is collision data (in my pure ignorance :) )
	ITEM_ENT=1800,//entity data/item
	SND_CONF=1900,//sound configuration file/text
	SND_LST=2000,//sound list
	SND_LST2=2100,//sound list again
	SND_TXT=2200,//sound text file - usage unknown
	SND_TXT2=2300,//sound text file 2 - usage unknown
	WEAPON_ENT=2400,//entity data/weapon
	ARMOUR_ENT=2500,//entity data/armour
	DOOR_ENT=2600,//entity data/door
	USE_ENT=2700,//entity data/self use (used on Player Character)
	AMMO_ENT=3500,//entity data/ammunition
	CON_ENT=3600,//entity data/unknown
	TXT=3700//text/tokens
};

struct Entry
{
	long one;//always 1 ????
	long number;//number of file in PACK file
	long type;//type of file as defined in ftype enum
	long offset;//string offset (offsetResources+offset)
	long pack;
};

struct F3GRPHeader
{
    long vMajor;
    long vMinor;
    long nFiles;//number of files in PACK
};

struct Lump
{
    long offset;
    long lenght;
};


void extractFile(char* packname,long filenumber,long filetype,char* filename)
{
	char			fname[]="1234567890123456789012345678901234567890123456789012345678901234567890";
	FILE*			hfile;
	FILE*			htemp;

	F3GRPHeader		head;

	Lump*			lumps;
	char*			buff;

	hfile = fopen(packname, "rb");

	if(hfile==0)
	{
		printf("Could not open file:%s\n",packname);
		return;
	}
	
	fread (&head, sizeof(head),1,hfile);

	if(head.vMajor!=2)
		return;

	lumps=new Lump[head.nFiles];

	fread (lumps, sizeof(Lump),head.nFiles,hfile);

	fseek(hfile, lumps[filenumber].offset, SEEK_SET);
	
	buff = new char[lumps[filenumber].lenght];
	fread(buff,lumps[filenumber].lenght,1,hfile);
	fclose( hfile);

	switch(filetype)
	{
	case G3D:
		sprintf(fname,"%s.b3d",filename);
		break;
	case TGA:
		sprintf(fname,"%s.tga",filename);
		break;
	case SMA:
		sprintf(fname,"%s.sma",filename);
		break;
	case TRE:
		sprintf(fname,"%s.8",filename);
		break;
	case SKEL:
		sprintf(fname,"%s.skel",filename);
		break;
	case ANIM:
		sprintf(fname,"%s.anim",filename);
		break;
	case INI:
		sprintf(fname,"%s.ini",filename);
		break;
	case WAV:
		sprintf(fname,"%s.wav",filename);
		break;
	case SCR:
		sprintf(fname,"%s.amx",filename);
		break;
	case MAP:
		sprintf(fname,"%s.map",filename);
		break;
	case CRITTER_ENT:
		sprintf(fname,"%s.crt",filename);
		break;
	case VFX:
		sprintf(fname,"%s.veg",filename);
		break;
	case GUI:
		sprintf(fname,"%s.int",filename);
		break;
	case COL:
		sprintf(fname,"%s.rle",filename);//unknown
		break;
	case ITEM_ENT:
		sprintf(fname,"%s.itm",filename);
		break;
	case SND_CONF:
		sprintf(fname,"%s.txt",filename);
		break;
	case SND_LST:
		sprintf(fname,"%s.lst",filename);
		break;
	case SND_LST2:
		sprintf(fname,"%s.lst",filename);
		break;
	case SND_TXT:
		sprintf(fname,"%s.txt",filename);
		break;
	case SND_TXT2:
		sprintf(fname,"%s.txt",filename);
		break;
	case WEAPON_ENT:
		sprintf(fname,"%s.wea",filename);
		break;
	case ARMOUR_ENT:
		sprintf(fname,"%s.arm",filename);
		break;
	case DOOR_ENT:
		sprintf(fname,"%s.dor",filename);
		break;
	case USE_ENT:
		sprintf(fname,"%s.use",filename);
		break;
	case AMMO_ENT:
		sprintf(fname,"%s.amo",filename);
		break;
	case CON_ENT:
		sprintf(fname,"%s.con",filename);//unknown
		break;
	case TXT:
		sprintf(fname,"%s.txt",filename);
		break;
	default:
		printf("ERROR\n");
	}

	htemp = fopen(fname, "wb");
	fwrite(buff, lumps[filenumber].lenght,1,htemp);
	fclose( htemp);

	delete []lumps;
	delete []buff;
}


int main(int argc, char* argv[])
{
	printf("FALLOUT 3 (VAN BUREN) Resource Extractor\n");

	char			fname[]="resource.rht";
	char			grpname[]="1234567890123456789012345678901234567890123456789012345678901234567890";
	char			extname[]="1234567890123456789012345678901234567890123456789012345678901234567890";
	FILE*			hfile;
	//FILE*			htemp;
	F3RHTHeader		head;
	Entry*			entries;

	long			flen;

	char*			names;
	char*			strings;

	int				i;

	hfile = fopen(fname, "rb");

	if(hfile==0)
	{
		printf("Could not open file:%s\n",fname);
		return -1;
	}

	fseek(hfile,0,SEEK_END);
	flen=ftell(hfile);
	fseek(hfile,0,SEEK_SET);
	
	fread (&head, sizeof(head),1,hfile);

	if(head.vMajor!=1)
	{
		printf("Invalid file type! %s\n",fname);
		return -1;
	}
	
	entries=new Entry[head.nEntries];
	names=new char[head.offsetResources-head.offsetPacks];
	strings=new char[flen-head.offsetResources];

	fread(entries,sizeof(Entry),head.nEntries,hfile);

	fseek(hfile,head.offsetPacks,SEEK_SET);
	fread(names,head.offsetResources-head.offsetPacks,1,hfile);
	

	fseek(hfile,head.offsetResources,SEEK_SET);
	fread(strings,flen-head.offsetResources,1,hfile);

	for(i=0;i<head.nEntries;i++)
	{
		printf("[%s]                                                 \nProgress:%3.2f%% \r",&strings[entries[i].offset],(float(i)/float(head.nEntries))*100);
		mkdir(&names[entries[i].pack]);
		sprintf(grpname,"data\\%s.grp",&names[entries[i].pack]);
		sprintf(extname,"%s\\%s",&names[entries[i].pack],&strings[entries[i].offset]);
		extractFile(grpname,entries[i].number,entries[i].type,extname);
	}

	fclose(hfile);

	printf("Finished extracting\n");

	delete [] names;
	delete [] strings;
	delete [] entries;

	return 0;
}
