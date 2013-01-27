FileStore project readme
========================

Objectives:
 - A simple file storage component which provides reliable access to files through a GUID id
 - Smart pointer based access to files, creating the appearance of copying while minimizing physical storage requirements
 - Distributed storage support, allowing splitting the store across multiple separate locations
 - Ability to handle both string paths and streams, to support efficient paths but also streamed uploads without saving as a temporary
 - No dependency on SQL or other external storage (interface decoupled)
 - No hastle unit testing