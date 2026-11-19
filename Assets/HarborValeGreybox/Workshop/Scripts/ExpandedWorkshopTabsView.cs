using System.Linq;
using UnityEngine;
namespace HarborValeGreybox.Workshop {
public sealed partial class ExpandedWorkshopView {
 public string ActiveTab=>category;
 public void SelectTab(string id){WorkshopTabs.Find(id);category=id;Search="";scroll=detailScroll=Vector2.zero;clearFocus=true;var visible=Visible();if(visible.Count>0&&!visible.Any(o=>o.id==garage.SelectedId))garage.SelectedId=visible[0].id;}
 public void Reveal(string offerId){SelectTab(WorkshopTabs.For(garage.Catalog.Find(offerId)));garage.SelectedId=offerId;}
 void DrawTabs(){GUI.Box(new Rect(24,112,1392,88),"",card);for(int i=0;i<WorkshopTabs.All.Length;i++){var tab=WorkshopTabs.All[i];int count=garage.Catalog.offers.Count(o=>WorkshopTabs.Contains(tab.id,o));var style=new GUIStyle(category==tab.id?primary:button){fontSize=16};if(GUI.Button(new Rect(32+(i%9)*154,120+(i/9)*40,146,32),tab.label+" · "+count,style))SelectTab(tab.id);}}
 void DrawCarCard(float y){var b=garage.Build;var current=garage.Catalog.Resolve(b);GUI.Box(new Rect(24,y,280,789-y),"",card);Label(new Rect(42,y+20,245,30),garage.IsTest?"ASTER APEX":"ASTER FINCH",25,Color.white,true);Label(new Rect(42,y+61,245,42),b.induction=="none"?(b.naStage==0?"STOCK ENGINE / NA":"NA ENGINE / STAGE "+b.naStage):b.induction.ToUpperInvariant()+" / Stage "+b.inductionStage,17,accent,true,true);RenderPreview();if(preview!=null)GUI.DrawTexture(new Rect(39,y+116,250,150),preview,ScaleMode.ScaleToFit,false);if(GUI.Button(new Rect(42,y+276,72,30),"←",button))angle-=25;if(GUI.Button(new Rect(215,y+276,72,30),"→",button))angle+=25;Label(new Rect(126,y+282,80,22),"BUILD",13,muted);
  Label(new Rect(42,y+321,245,30),ChassisTuning.CrankHp(current).ToString("F0")+" HP / "+ChassisTuning.EstimatedWhp(current).ToString("F0")+" WHP est",19,Color.white,true);Label(new Rect(42,y+356,245,25),"Vehicle mass "+current.massKg.ToString("N0")+" kg",18,muted);Label(new Rect(42,y+385,245,24),current.PeakKw().ToString("F1")+" kW / "+current.torque.Max().ToString("F0")+" Nm",16,muted);Label(new Rect(42,y+413,245,22),FitmentTuning.Drive(garage.Catalog,b).ToUpperInvariant()+" / "+b.tireCompound.ToUpperInvariant()+" TIRES S"+b.tireStage,14,accent,true);Label(new Rect(42,y+441,245,20),"PART PREVIEW",14,muted);if(partPreview==null)partPreview=new WorkshopPartPreview();var selected=Visible().FirstOrDefault(o=>o.id==garage.SelectedId);partPreview.Draw(new Rect(39,y+464,250,80),selected);Label(new Rect(42,768,245,20),garage.IsTest?"Temporary test changes":"Payments and build saved together",13,muted);
 }
}
}
