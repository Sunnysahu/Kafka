**Topic → Partition → Offset**

---

 ## 1. Start Docker

 From the Kafka project folder, start the containers:

```
docker compose up -d
```

 Verify that the containers are running:

```
docker ps
```

 You should see the Kafka container running, for example:

```
shipment-kafka
```

---

 ## 2\. Create `shipment-events`

 Create a Kafka topic named `shipment-events` with **3 partitions**:

```
docker exec -it shipment-kafka /opt/kafka/bin/kafka-topics.sh \
  --create \
  --topic shipment-events \
  --bootstrap-server localhost:9092 \
  --partitions 3 \
  --replication-factor 1
```

 The important options are:

 | Option | Meaning |
| --- | --- |
| `--topic shipment-events` | Name of the topic |
| `--partitions 3` | Create 3 partitions |
| `--replication-factor 1` | Store one replica of each partition |
| `--bootstrap-server localhost:9092` | Kafka broker to connect to |

We intentionally use **3 partitions** because partitioning will become important when we discuss:

 - Parallel consumers
- Consumer groups
- Scaling
- Ordering
- Message keys

---

 ## 3\. Verify the Topic

 Describe the topic:

```
docker exec -it shipment-kafka /opt/kafka/bin/kafka-topics.sh \
  --describe \
  --topic shipment-events \
  --bootstrap-server localhost:9092
```

 You should see output similar to:

```
Topic: shipment-events
PartitionCount: 3
ReplicationFactor: 1

Partition: 0
Partition: 1
Partition: 2
```

 You may also see additional information such as:

```
Leader
Replicas
Isr
```

 Don't worry about those details yet. We'll cover them when we discuss Kafka replication.

---

 # 4\. Open Kafka UI

 Open Kafka UI in your browser:

```
http://localhost:8080/
```

 You should see something similar to:

```
shipment-kafka
    │
    └── Topics
         │
         └── shipment-events
              ├── Partition 0
              ├── Partition 1
              └── Partition 2
```

---

 # 5\. Why Three Partitions?

 Imagine that eventually we have:

```
TrackingService
     │
     └── Consumer Group
          ├── Consumer 1
          ├── Consumer 2
          └── Consumer 3
```

 Our topic has:

```
shipment-events
       │
       ├── Partition 0
       ├── Partition 1
       └── Partition 2
```

 Kafka can distribute the partitions between consumers:

```
Partition 0 ──> Consumer 1

Partition 1 ──> Consumer 2

Partition 2 ──> Consumer 3
```

 This allows consumers in the same consumer group to process different partitions in parallel.

 ### Important

 More partitions **does not automatically mean better performance**.

 The number of partitions affects things such as:

 - Consumer parallelism
- Ordering
- Throughput
- Consumer-group scaling
- Broker resources

 We'll explore these trade-offs later.

---

 # 6\. A Topic Is Not One Queue

 One of the most important Kafka concepts is:

 > A Kafka topic is made up of partitions.

 Our topic:

```
shipment-events
       │
       ├── Partition 0
       ├── Partition 1
       └── Partition 2
```

 Each partition maintains its **own sequence of offsets**.

 For example:

```
Partition 0
------------
Offset
  0
  1
  2
  3
```

```
Partition 1
------------
Offset
  0
  1
  2
```

```
Partition 2
------------
Offset
  0
  1
```

 Notice that each partition starts its own offset sequence.

---

 # 7\. There Is No Single Global Offset

 This is an important Kafka concept.

 There is **not** one global offset for the entire topic.

 Instead, offsets belong to individual partitions.

 For example:

```
shipment-events

Partition 0
    Offset 0
    Offset 1
    Offset 2
    Offset 3

Partition 1
    Offset 0
    Offset 1
    Offset 2

Partition 2
    Offset 0
    Offset 1
```

 Therefore:

```
Partition 0 → Offset 3
Partition 1 → Offset 2
Partition 2 → Offset 1
```

 These offsets are independent of each other.

---

 # 8\. Topic → Partition → Offset

 Keep this mental model in mind:

```
Topic
  │
  └── shipment-events
        │
        ├── Partition 0
        │      ├── Offset 0
        │      ├── Offset 1
        │      └── Offset 2
        │
        ├── Partition 1
        │      ├── Offset 0
        │      ├── Offset 1
        │      └── Offset 2
        │
        └── Partition 2
               ├── Offset 0
               └── Offset 1
```

 Think of it as:

```
Topic
  ↓
Partitions
  ↓
Messages
  ↓
Offsets
```

 A message is therefore associated with a **partition and an offset**.

---

 # 9\. What We'll Learn Next

 Now that we have created:

```
shipment-events
```

 with:

```
3 partitions
```

 we can move on to producing messages.

 The next step will be to create our **.NET Kafka producer** and see how messages are actually written into these partitions.

 Later, we'll also investigate how Kafka decides which partition receives a message, including the role of:

 - Message keys
- Partitioners
- Ordering
- Consumer groups
- Offsets

---

 # Quick Reference

 ### Start Kafka

```
docker compose up -d
```

 ### Create topic

```
docker exec -it shipment-kafka /opt/kafka/bin/kafka-topics.sh \
  --create \
  --topic shipment-events \
  --bootstrap-server localhost:9092 \
  --partitions 3 \
  --replication-factor 1
```

 ### Describe topic

```
docker exec -it shipment-kafka /opt/kafka/bin/kafka-topics.sh \
  --describe \
  --topic shipment-events \
  --bootstrap-server localhost:9092
```

 ### Kafka UI

```
http://localhost:8080/
```

---

 ## Key Takeaways

 1. A **topic** is divided into one or more partitions.
2. Our `shipment-events` topic has **3 partitions**.
3. Each partition has its own **offset sequence**.
4. There is **no single global offset** for a topic.
5. Partitions allow Kafka consumers to process messages in parallel.
6. More partitions do not automatically mean better performance.
7. Message keys can influence which partition receives a message.
8. Partitioning will become especially important when we introduce **consumer groups and scaling**.