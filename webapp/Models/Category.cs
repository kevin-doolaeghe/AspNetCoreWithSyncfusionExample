using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.Models {

    public class Category {

        [Key]
        public long Id { get; set; }

        public string? UserId { get; set; }

        public User? User { get; set; }

        public string? Icon { get; set; }

        public string? Name { get; set; }

        [NotMapped]
        public string Description => $"{Icon} {Name}";

        [NotMapped]
        public static readonly List<string> Icons = new() {
            // Money
            "💰", "💸", "💳", "💶", "💵", "💲", "🏛️", "🤑",
            // Food
            "🍔", "🍕", "🥪", "🥩", "🥐", "🍩", "🍌",
            // Energy
            "💡", "⚡", "🔥", "💧", "🌿", "☀️", "♻️", "⛽",
            // Shopping
            "🧾", "🏷️", "🧺", "🛒", "🛍️", "📦",
            // Transport
            "🚗", "🏍️", "🚌", "🚕", "🏎️", "🚲",
            // Health
            "❤️", "🧠", "💉", "💊", "🔬", "🏥", "🤒", "🤕",
            // Travel
            "🧳", "⛱️", "🗻", "🏕", "🗺️", "🏄",
            // Pleasure
            "⚽", "🎸", "🎥", "🍿", "🎳", "🎢", "😎",
            // Happening
            "🎂", "🎁", "🎉", "🍾", "🥳",
            // Technology
            "📺", "📱", "💻", "🕹️", "🎮", "💾",
            // Sport
            "🎾", "🏐", "🏆", "🏅", "🥅", "🏟️", "💪", "⛹️", "🏋️",
            // House
            "🔨", "🔧", "🔑", "🌱", "🛏️", "🏡", "👪", "🐶",
            // Office
            "💼", "📖", "📃", "📚", "🖊️", "📨", "🏢", "👨‍💼",
            // School
            "📒", "🎒", "🎓", "🏫", "🧑‍🏫",
            // Positive
            "💕", "⭐", "✨", "🌈", "🎈", "📣", "😊",
            // Negative
            "💣", "💥", "⚠️", "⛔", "❗", "💀", "🤯",
            // Other
            "✔️", "❌", "➕", "➖", "🔗", "🔒",
        };
    }
}
