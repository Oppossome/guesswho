using guesswho.walker;
using Sandbox;
using System;
using System.Collections.Generic;


namespace guesswho
{
	public class Outfit
	{
		public string Shirt { get; set; }
		public string Pants { get; set; }
		public string Shoes { get; set; }
		public string Hat { get; set; }

		public float Height { get; set; }
		public int Skin { get; set;}
		
		public List<ModelEntity> Clothing = new();
		Entity Owner { get; set;}

		public Outfit(Entity ent, Outfit outfit)
		{
			Owner = ent;

			Height = outfit.Height;
			Skin = outfit.Skin;

			Shirt = outfit.Shirt;
			Pants = outfit.Pants;
			Shoes = outfit.Shoes;
			Hat = outfit.Hat;
		}

		public Outfit(Entity ent)
		{
			Owner = ent;

			Height = 0.95f + Rand.Float(0f, 0.1f);
			Skin = Rand.Int(0, 3);

			Hat = Rand.FromArray(new[]
			{
				"models/citizen_clothes/hat/hat_hardhat.vmdl",
				"models/citizen_clothes/hat/hat_woolly.vmdl",
				"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
				"models/citizen_clothes/hair/hair_malestyle02.vmdl",
				"models/citizen_clothes/hair/hair_femalebun.black.vmdl",
				"models/citizen_clothes/hat/hat_beret.red.vmdl",
				"models/citizen_clothes/hat/hat.tophat.vmdl",
				"models/citizen_clothes/hat/hat_beret.black.vmdl",
				"models/citizen_clothes/hat/hat_cap.vmdl",
				"models/citizen_clothes/hat/hat_leathercap.vmdl",
				"models/citizen_clothes/hat/hat_leathercapnobadge.vmdl",
				"models/citizen_clothes/hat/hat_securityhelmetnostrap.vmdl",
				"models/citizen_clothes/hat/hat_service.vmdl",
				"models/citizen_clothes/hat/hat_uniform.police.vmdl",
				"models/citizen_clothes/hat/hat_woollybobble.vmdl",
			});

			Shirt = Rand.FromArray(new[] {
				"models/citizen_clothes/jacket/labcoat.vmdl",
				"models/citizen_clothes/jacket/jacket.red.vmdl",
				"models/citizen_clothes/jacket/jacket.tuxedo.vmdl",
				"models/citizen_clothes/jacket/jacket_heavy.vmdl",
			});

			Pants = Rand.FromArray(new[]{
				"models/citizen_clothes/trousers/trousers.jeans.vmdl",
				"models/citizen_clothes/trousers/trousers.lab.vmdl",
				"models/citizen_clothes/trousers/trousers.police.vmdl",
				"models/citizen_clothes/trousers/trousers.smart.vmdl",
				"models/citizen_clothes/trousers/trousers.smarttan.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuit.vmdl",
				"models/citizen_clothes/shoes/shorts.cargo.vmdl",
			});

			Shoes = Rand.FromArray(new[]
			{
				"models/citizen_clothes/shoes/trainers.vmdl",
				"models/citizen_clothes/shoes/shoes.workboots.vmdl"
			});
		}

		public void ApplyOutfit()
		{
			ModelEntity ownerModel = Owner as ModelEntity;
			ownerModel.SetMaterialGroup(Skin);
			ownerModel.Scale = Height;

			ModelEntity shirtEnt = new ModelEntity(Shirt, Owner);
			shirtEnt.SetMaterialGroup(Skin);
			shirtEnt.Tags.Add("clothing");
			Clothing.Add(shirtEnt);

			ModelEntity pantsEnt = new ModelEntity(Pants, Owner);
			ownerModel.SetBodyGroup("Legs", 1);
			pantsEnt.SetMaterialGroup(Skin);
			pantsEnt.Tags.Add("clothing");
			Clothing.Add(pantsEnt);
			

			ModelEntity shoesEnt = new ModelEntity(Shoes, Owner);
			ownerModel.SetBodyGroup("Feet", 1);
			shoesEnt.SetMaterialGroup(Skin);
			shoesEnt.Tags.Add("clothing");
			Clothing.Add(shoesEnt);

			ModelEntity hatEnt = new ModelEntity(Hat, Owner);
			hatEnt.Tags.Add("clothing");
			Clothing.Add(hatEnt);
		}

		public void Clear()
		{
			Clothing.ForEach((x) => x.Delete());
		}
	}
}
