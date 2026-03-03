namespace GameCharacterSystem{
    class Character
    {
        public string Name { get; set; }
        public virtual void Attack()
        {
            Console.WriteLine(Name + " attacks!");
        }
    }

    class Warrior : Character
    {
        public override void Attack()
        {
            Console.WriteLine(Name + " swings a sword!");
        }
    }

    class Mage : Character
    {
        public override void Attack()
        {
            Console.WriteLine(Name + " casts a fireball!");
        }
    }

    class Program
    {
        static void Main()
        {
            Character c1 = new Warrior { Name = "Thor" };
            Character c2 = new Mage { Name = "Merlin" };
            c1.Attack();
            c2.Attack();
        }
    }
}
