# WeatherStation

## Interesting Queries

### Recursively get all symptoms of the common cold
g.V().hasLabel('condition').has('name', 'Common Cold').repeat(inE('symptom').outV()).emit()
