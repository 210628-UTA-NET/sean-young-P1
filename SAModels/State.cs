using System.ComponentModel.DataAnnotations;


namespace SAModels {
    public class State : IStoreModel {
        [Key]
        [Required]
        [MaxLength(2)]
        public string Code { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
