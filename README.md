# GeneticHelloWorld
Genetic algorithm hello world in C#

Based on http://www.generation5.org/content/2003/gahelloworld.asp and http://www.puremango.co.uk/2010/12/genetic-algorithm-for-hello-world/

Start with random population then loop:
  1. sort by best fit (0 = exact fit, the higher the number less fit it is), if that one is 0 then break (we're done)
  2. print best fit (value and fit score)
  3. use best fits to generate (mate) several random ones (offspring), also include the current one (since if it is better, we want random strings from that one again)
