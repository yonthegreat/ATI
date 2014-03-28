function findDiv(n, d)
{
	var p,i,x;  if(!d) d=document; if((p=n.indexOf("?"))>0&&parent.frames.length)
	{
    d=parent.frames[n.substring(p+1)].document; n=n.substring(0,p);
	}
  if(!(x=d[n])&&d.all) x=d.all[n]; for (i=0;!x&&i<d.forms.length;i++) x=d.forms[i][n];
  for(i=0;!x&&d.layers&&i<d.layers.length;i++) x=findDiv(n,d.layers[i].document);
  if(!x && document.getElementById) x=document.getElementById(n); return x;
}

function toggleNav()
{
	var i,p,v,obj,args=toggleNav.arguments;
  for (i=0; i<(args.length-2); i+=3) if ((obj=findDiv(args[i]))!=null) { v=args[i+2];
    if (obj.style) { obj=obj.style; v=(v=='show')?'visible':(v='hide')?'hidden':v; }
    obj.visibility=v; }
}

function hideAllDDs()
{
	//toggleNav('homeDiv','','hide');
	toggleNav('youraccountDiv','','hide');
	toggleNav('yourhomeDiv','','hide');
	toggleNav('yourbusinessDiv','','hide');
	toggleNav('benefitsofgasDiv','','hide');
	toggleNav('safetyDiv','','hide');
}

function hideDD(sect)
{
	toggleNav(sect,'','hide');
	hideAllDDs();
}

var sectionSetting = null;
var activeTimer = null;
var bHover = '';
function setDDactive(sect, windowStatusVal, bshow)
{
	hideAllDDs();
	if ((bHover==windowStatusVal)||(bshow))
	{
		toggleNav(sect,'','show');
		if (sectionSetting != null)
		{
				window.clearTimeout(sectionSetting);
		}
	}
}

function setDDTimeout(sect)
{
	if (sectionSetting != null)
	{
		window.clearTimeout(sectionSetting);
	}
	sectionSetting = window.setTimeout('hideDD("' + sect + '")',500);
}

/*
function hideSelect(){
	if (document.all){
		document.all.formselect.style.visibility="hidden";
	}
}

function unhideSelect(){
	if (document.all){
		document.all.formselect.style.visibility="visible";
	}
}

function hideSelect()
{
     var selCount = document.all.tags("select");

     for (i=0; i<selCount.length; i++)

          selCount[i].style.visibility = "hidden";
}

function unhideSelect() {

 selCount=document.all.tags("select")

     for (i=0;i<selCount.length;i++)

          selCount[i].style.visibility="visible"

}
*/
