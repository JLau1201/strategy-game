

**Mushroomology 120: Introduction to Mushrooms**

Group funGuys

YuSong Wang (301457147)  
Jerome Lau (301411165)  
HaoHan Xu (301416880)  
Simon Fraser University  
10/8/2024

**Dataset**

[https://archive.ics.uci.edu/dataset/848/secondary+mushroom+dataset](https://archive.ics.uci.edu/dataset/848/secondary+mushroom+dataset)

| Variable | Type | Description | Values |
| ----- | ----- | ----- | ----- |
| cap-diameter | M | Cap diameter in cm | Float number in cm |
| cap-shape | N | Shape of the cap | Bell \= `b`, Conical \= `c`, Convex \= `x`, Flat \= `f`, Sunken \= `s`, Spherical \= `p`, Others \= `o` |
| cap-surface | N | Surface texture of the cap | Fibrous \= `i`, Grooves \= `g`, Scaly \= `y`, Smooth \= `s`, Shiny \= `h`, Leathery \= `l`, Silky \= `k`, Sticky \= `t`, Wrinkled \= `w`, Fleshy \= `e` |
| cap-color | N | Color of the cap | Brown \= `n`, Buff \= `b`, Gray \= `g`, Green \= `r`, Pink \= `p`, Purple \= `u`, Red \= `e`, White \= `w`, Yellow \= `y`, Blue \= `l`, Orange \= `o`, Black \= `k` |
| does-bruise-bleed | N | Whether the mushroom bruises/bleeds | Bruises or Bleeding \= `t`, No \= `f` |
| gill-attachment | N | Attachment type of the gills | Adnate \= `a`, Adnexed \= `x`, Decurrent \= `d`, Free \= `e`, Sinuate \= `s`, Pores \= `p`, None \= `f`, Unknown \= `?` |
| gill-spacing | N | Spacing between the gills | Close \= `c`, Distant \= `d`, None \= `f` |
| gill-color | N | Color of the gills | See cap-color \+ None \= `f` |
| stem-height | M | Height of the stem in cm | Float number in cm |
| stem-width | M | Width of the stem in mm | Float number in mm |
| stem-root | N | Type of stem root | Bulbous \= `b`, Swollen \= `s`, Club \= `c`, Cup \= `u`, Equal \= `e`, Rhizomorphs \= `z`, Rooted \= `r` |
| stem-surface | N | Surface texture of the stem | See cap-surface \+ None \= `f` |
| stem-color | N | Color of the stem | See cap-color \+ None \= `f` |
| veil-type | N | Type of the veil | Partial \= `p`, Universal \= `u` |
| veil-color | N | Color of the veil | See cap-color \+ None \= `f` |
| has-ring | N | Presence of a ring | Ring \= `t`, None \= `f` |
| ring-type | N | Type of ring | Cobwebby \= `c`, Evanescent \= `e`, Flaring \= `r`, Grooved \= `g`, Large \= `l`, Pendant \= `p`, Sheathing \= `s`, Zone \= `z`, Scaly \= `y`, Movable \= `m`, None \= `f`, Unknown \= `?` |
| spore-print-color | N | Color of the spore print | See cap-color |
| habitat | N | Habitat of the mushroom | Grasses \= `g`, Leaves \= `l`, Meadows \= `m`, Paths \= `p`, Heaths \= `h`, Urban \= `u`, Waste \= `w`, Woods \= `d` |
| season | N | Season when the mushroom grows | Spring \= `s`, Summer \= `u`, Autumn \= `a`, Winter \= `w` |
| class | Binary | Edibility of the mushroom | Edible \= `e`, Poisonous \= `p` (includes unknown edibility) |

**Problem Definition & Justification**

For many people, foraging for mushrooms is a dangerous task. While some mushrooms are absolutely delicious like the Shiitake mushrooms, others can be poisonous or even deadly, such as the Death Cap Mushrooms. As more and more people cherish a healthy lifestyle of eating organic foods, mushroom-picking will undoubtedly be on a rising trend. Therefore, it is crucial to know which mushrooms are safe for consumption, and which mushrooms are harmful. Identifying the differences based on visual features like colour, cap shape, gill attachment, or habitat can be extremely challenging without guidance. 

The dataset we chose for our project includes various mushroom features such as cap shape, surface texture, gill attachment, stem height, and habitat. Our project aims to develop a classification model that can assist inexperienced foragers in making informed decisions on determining whether a mushroom is edible or not. Such a model could serve as a useful tool for foragers, mycologists, and even food safety researchers, helping to demystify mushroom identification, reducing the risk of accidental poisoning.

The selected mushroom dataset is a real world dataset containing over 60000 samples, satisfying the requirement for a dataset with at least 1,000 samples. It also includes 20 features that cover a diverse range of categorical (habitat, colour) and numerical variables (cap diameter, stem height). In addition, the dataset includes a target variable: Class, which is a binary variable with two possible values: e (edible) or p (poisonous). Class is a target variable because every other feature can be used as a predictor to a mushroom’s edibility. 

**Data Preprocessing \- jerome**

| Attribute | Number of Missing Values | Action |
| :---: | :---: | :---: |
| cap-diameter | 0 |  |
| cap-shape | 0 |  |
| cap-surface | 14120 |  |
| cap-color | 0 |  |
| does-bruise-bleed | 0 |  |
| gill-attachment | 9884 |  |
| gill-spacing | 25063 |  |
| gill-color | 0 |  |
| stem-height | 0 |  |
| stem-width | 0 |  |
| stem-root | 51538 | Remove Attribute |
| stem-surface | 38124 | Remove Attribute |
| stem-color | 0 |  |
| veil-type | 57892 | Remove Attribute |
| veil-color | 53656 | Remove Attribute |
| has-ring | 0 |  |
| ring-type | 2471 |  |
| spore-print-color | 54715 | Remove Attribute |
| habitat | 0 |  |
| season | 0 |  |

| Attribute | Z-score |  |  | IQR |  |  |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: |
|  |  | LB | UB |  | LB | UB |
| cap-diameter | 521 | N/A | 22.53 | 2400 | N/A | 16.13 |
| stem-height | 1254 | N/A | 16.69 | 3169 | N/A | 12.39 |
| stem-width | 857 | N/A | 42.26 | 1967 | N/A | 33.61 |

**Exploratory Data Analysis (EDA) \- eric**

We used the .info() function from pandas to obtain a summary of the dataset, including information such as type of data, and the number of Non-Null values. We also used the .describe() function from pandas to obtain the count, mean, standard deviation, minimum, 25th Percentile, 50th Percentile, 75th Percentile, and maximum of the 3 numerical attributes. 

We used histograms to display the frequency of all the different values for each categorical attribute, and we used boxplots to display the frequency of all the different values for each numerical attribute. Lastly, we used heatmaps to find correlation between numerical attributes. 

**Clustering \- eric**

**Outlier Detection \- fred**

**Feature Selection \- fred**

**Classification \- jerome**

**Hyperparameter Tuning \- jerome**

**Conclusion**

