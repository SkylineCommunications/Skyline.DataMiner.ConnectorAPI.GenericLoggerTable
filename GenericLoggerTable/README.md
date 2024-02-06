# Skyline.DataMiner.ConnectorAPI.GenericLoggerTable

## Description

This NuGet package is used to access data from a [Logger Table](https://docs.dataminer.services/develop/devguide/Connector/AdvancedLoggerTables.html) hosted by a DataMiner element.

Requirements:
- DataMiner system with [Generic Logger Table connector](https://catalog.dataminer.services/details/connector/8410) element

## Getting Started

- Reference the NuGet package from an Automation script, Connector, GQI query,... 
- Create a new instance of a `GenericLoggerTableElement` by passing the `connection`, `agentId` and `elementId`
- Store and retrieve data using the CRUD operations available from the `GenericLoggerTableElement` instance.

## Typical Use Case

Logger Tables are used to quickly store and retrieve large pieces of data. Note that this data is written immediately to the underlying database. As such, there's no way to monitor the data from DataMiner.

## About

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal with world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.

<!-- Uncomment below and add more info to provide more information about how to use this package. -->
<!-- ## Getting Started -->
