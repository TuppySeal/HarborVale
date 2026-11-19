using System;
namespace HarborValeGreybox.Workshop {
public static class WorkshopTabs {
 public sealed class Tab {public readonly string id,label;public Tab(string id,string label){this.id=id;this.label=label;}}
 public static readonly Tab[] All={new Tab("all","All parts"),new Tab("na","Engine tunes"),new Tab("small","Small turbo"),new Tab("large","Large turbo"),new Tab("roots","Supercharger"),new Tab("twin","Twin turbo"),new Tab("cooling","Intercoolers"),new Tab("gearbox","Gearbox"),new Tab("brakes","Brakes"),new Tab("tires","Tires"),new Tab("intake","Intake"),new Tab("exhaust","Exhaust"),new Tab("sound","Sound tunes"),new Tab("differential","Differential"),new Tab("suspension","Suspension"),new Tab("gearing","Final drive"),new Tab("aero","Aero"),new Tab("drivetrain","Drivetrain")};
 public static string For(WorkshopOffer offer)=>offer.slot=="tirecompound"?"tires":offer.slot=="induction"?(offer.choice=="none"?"na":offer.choice):offer.slot;
 public static bool Contains(string tab,WorkshopOffer offer)=>tab=="all"||For(offer)==tab;
 public static Tab Find(string id){foreach(var tab in All)if(tab.id==id)return tab;throw new ArgumentException("Unknown workshop tab: "+id);}
}
}
