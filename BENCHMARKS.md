## Field access

| Access type | Elapsed time (ticks/100,000 accesses) | Ratio vs. compiled |
| - | - | - |
| Compiled, Field | 37514.5 | 1x |
| Generic Getter, Field | 42212 | 1.125x |
| Specific Getter, Field | 43203.8 | 1.512x |
| Property Getter, Field | 46247 | 1.233x |
| Reflected Getter, Field | 317665 | 8.468x |

## Property access

| Access type | Elapsed time (ticks/100,000 accesses) | Ratio vs. compiled |
| - | - | - |
| Compiled, Property | 42863 | 1x |
| Generic Getter, Property | 49247.1 | 1.149x |
| Specific Getter, Property | 51104.5 | 1.192x |
| Property Getter, Property | 53829.1 | 1.256x |
| Reflected Getter, Property | 156328.8 | 3.647x |

## Method access

| Access type | Elapsed time (ticks/100,000 accesses) | Ratio vs. compiled |
| - | - | - |
| Compiled, Method | 40749.9 | 1x |
| Generic Getter, Method | 50659.3 | 1.243x |
| Specific Getter, Method | 53297 | 1.309x |
| Property Getter, Method | 54570.3 | 1.339x |
| Reflected Getter, Method | 895844 | 21.984x |
