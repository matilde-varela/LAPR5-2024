:- module(config, [
    reference_value/1,
    time_limit/1,
    population/1,
    generations/1,
    prob_crossover/1,
    prob_mutation/1,
    mix_percentage/1
]).

reference_value(1100).
time_limit(10).
population(2).
generations(10).
prob_crossover(0.5).
prob_mutation(0.25).
mix_percentage(0.2).